using Microsoft.Data.SqlClient;
using Thiskord_Back.Models;
using Thiskord_Back.Services;

namespace Thiskord_Back.Services
{
    public interface IInviteService
    {
        Task<bool> AcceptInvite(string token);
        Task<string> CreateInvite(int projectId, int creatorId, DateTime? expiresAt);
    }
    
    public class InviteService : IInviteService
    {
        private readonly IDbConnectionService _dbService;
        private readonly ILogService _logService;

        public InviteService(IDbConnectionService dbService, ILogService logService)
        {
            _dbService = dbService;
            _logService = logService;
        }
        public async Task<bool> AcceptInvite(string token)
        {
            try
            {
                using var conn = _dbService.CreateConnection();
                await conn.OpenAsync();

                const string query = @"
                    UPDATE dbo.Invitation_Token
                    SET expires_at = GETDATE()
                    OUTPUT INSERTED.it_project_id, INSERTED.it_creator_id
                    WHERE it_token = @token
                      AND (expires_at IS NULL OR expires_at > GETDATE());";

                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@token", token);
                using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    int projectId = reader.GetInt32(0);
                    int creatorId = reader.GetInt32(1);

                    return true;
                }
                else
                {
                    return false; 
                }
            }
            catch (Exception e)
            {
                _logService.CreateLog($"Error in AcceptInvite: {e.Message}");
                throw;
            }
        }

        public async Task<string> CreateInvite(int projectId, int creatorId, DateTime? expiresAt)
        {

            try
            {
                if (expiresAt.HasValue && expiresAt.Value < DateTime.UtcNow)
                    throw new ArgumentException("La date d'expiration ne peut pas être dans le passé.",
                        nameof(expiresAt));

                using var conn = _dbService.CreateConnection();
                await conn.OpenAsync();
                Invitation invite = new Invitation()
                {
                    token = Guid.NewGuid().ToString("N"),
                    projectId = projectId,
                    creatorId = creatorId,
                    createdAt = DateTime.UtcNow,
                    expiresAt = expiresAt
                };

                const string query = @"
                INSERT INTO dbo.Invitation_Token (it_token, it_project_id, it_creator_id, created_at, expires_at)
                VALUES (@token, @projectId, @creatorId, @createdAt, @expiresAt);";

                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@token", invite.token);
                cmd.Parameters.AddWithValue("@projectId", invite.projectId);
                cmd.Parameters.AddWithValue("@creatorId", invite.creatorId);
                cmd.Parameters.AddWithValue("@createdAt", invite.createdAt);
                cmd.Parameters.AddWithValue("@expiresAt", (object)invite.expiresAt ?? DBNull.Value);
                await cmd.ExecuteNonQueryAsync();

                //return $"https://api.emre-ak.fr/invite/{invite.token}";
                return $"https://localhost:8080/invite/{invite.token}";
            }
            catch (Exception e)
            {
                _logService.CreateLog($"Error in CreateInvite: {e.Message}");
                throw;
            }
        }
    }    
}

