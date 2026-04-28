using Microsoft.Data.SqlClient;
using Thiskord_Back.Models;
using Thiskord_Back.Services;

namespace Thiskord_Back.Services
{
    public interface IInviteService
    {
        Task<bool> AcceptInvite(string token, int currentUserId);
        Task<string> CreateInvite(int projectId, int creatorId, string? expiresAt);
    }
    
    public class InviteService : IInviteService
    {
        private readonly IDbConnectionService _dbService;
        private readonly ILogService _logService;
        private readonly IConfiguration _config;

        public InviteService(IDbConnectionService dbService, ILogService logService, IConfiguration config)
        {
            _dbService = dbService;
            _logService = logService;
            _config = config;
        }
        public async Task<bool> AcceptInvite(string token, int currentUserId)
        {
            try
            {
                using var conn = _dbService.CreateConnection();
                await conn.OpenAsync();
                const string query = @"
                    UPDATE dbo.Invitation_Token
                    SET expires_at = '2050-01-01'
                    OUTPUT INSERTED.it_project_id
                    WHERE it_token = @token
                      AND (expires_at IS NULL OR expires_at > GETDATE());";

                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@token", token);
                using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    int projectId = reader.GetInt32(0);
                    await reader.CloseAsync();
                    
                    const string checkMember = @"
                    SELECT COUNT(1) FROM dbo.ACCESS 
                    WHERE id_account = @currentUserId AND id_project_account = @projectId";
                
                    using (var checkCmd = new SqlCommand(checkMember, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@projectId", projectId);
                        checkCmd.Parameters.AddWithValue("@currentUserId", currentUserId);

                        var count = Convert.ToInt32(await checkCmd.ExecuteScalarAsync());
                        if (count > 0) return true;
                    }
                    
                    const string query2 = @"
                        INSERT INTO dbo.ACCESS (is_admin, is_root, id_account, id_project_account) VALUES (0, 0, @currentUserId , @projectId);";
                    await using var cmd2 = new SqlCommand(query2, conn);
                    cmd2.Parameters.AddWithValue("@currentUserId", currentUserId);
                    cmd2.Parameters.AddWithValue("@projectId", projectId);
                    await cmd2.ExecuteNonQueryAsync();
                    return true;
                }

                return false;
            }
            catch (Exception e)
            {
                _logService.CreateLog($"Error in AcceptInvite: {e.Message}");
                throw;
            }
        }

        public async Task<string> CreateInvite(int projectId, int creatorId, string? expireAt)
        {
            DateTime? expiresAt = DateTime.TryParse(expireAt, out var parsed) ? parsed : null;
            using var conn = _dbService.CreateConnection();
            await conn.OpenAsync();
            const string isUserAdminQuery = @"
                SELECT is_admin, is_root 
                FROM dbo.ACCESS 
                WHERE id_account = @creatorId AND id_project_account = @projectId";
            await using (var cmd1 = new SqlCommand(isUserAdminQuery, conn))
            {
                cmd1.Parameters.AddWithValue("@creatorId", creatorId);
                cmd1.Parameters.AddWithValue("@projectId", projectId);
                using var reader = await cmd1.ExecuteReaderAsync();
                if (!await reader.ReadAsync()) 
                    return "Utilisateur non membre du projet";

                bool isAdmin = reader.GetBoolean(0);
                bool isRoot  = reader.GetBoolean(1);

                if (!isAdmin && !isRoot)
                    return "Utilisateur non administrateur du projet";
            }

            try
            {
                if (expiresAt.HasValue && expiresAt.Value < DateTime.UtcNow)
                    throw new ArgumentException("La date d'expiration ne peut pas être dans le passé.",
                        nameof(expiresAt));
                
                const string checkQuery = @"
                SELECT TOP 1 it_token 
                FROM dbo.Invitation_Token 
                WHERE it_project_id = @projectId 
                  AND it_creator_id = @creatorId 
                  AND (expires_at IS NULL OR expires_at > GETUTCDATE())";
                
                using (var checkCmd = new SqlCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@projectId", projectId);
                    checkCmd.Parameters.AddWithValue("@creatorId", creatorId);

                    var existingToken = await checkCmd.ExecuteScalarAsync() as string;
                    if (existingToken != null)
                        return $"{_config["AppSettings:BaseUrl"]}/api/invite/{existingToken}"; 
                        //return $"http://localhost:8080/api/invite/{existingToken}";
                }
                
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

                return $"{_config["AppSettings:BaseUrl"]}/api/invite/{invite.token}";
                //return $"http://localhost:8080/api/invite/{invite.token}";
            }
            catch (Exception e)
            {
                _logService.CreateLog($"Error in CreateInvite: {e.Message}");
                throw;
            }
        }
    }    
}

