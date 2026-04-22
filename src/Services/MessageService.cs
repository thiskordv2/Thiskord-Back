using Microsoft.Data.SqlClient;
using Thiskord_Back.Models;

namespace Thiskord_Back.Services
{
    
    public interface IMessageService
    {
        Task<List<MessageDTO>> GetAllMessage(int channelId);
        Task<MessageDTO> SendMessage(int channelId, int userId, string message, string username);
        Task DeleteMessage(int messageId, int channelId);
        Task<MessageDTO> UpdateMessage(int messageId, int channelId, string newContent);
    }
    
    public class MessageService : IMessageService
    {
        private readonly IDbConnectionService _dbService;
        private readonly ILogService logService;

        public MessageService(IDbConnectionService dbService, ILogService logService)
        {
            this._dbService = dbService;
            this.logService = logService;
        }

        public async Task<MessageDTO> SendMessage(int channelId, int userId, string message, string username)
        {
            try
            {
                using var conn = _dbService.CreateConnection();
                await conn.OpenAsync();

                const string query = @"
                    INSERT INTO dbo.Message (id_channel_author, id_author, message_content)
                    OUTPUT INSERTED.message_id
                    VALUES (@channelId, @userId, @text);";

                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@channelId", channelId);
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@text", message);
                var id = (int)await cmd.ExecuteScalarAsync();

                var newMessage = new MessageDTO(
                    username: username,
                    idMessage: id,
                    content: message,
                    createdAt: DateTime.UtcNow.ToString("dd/MM HH:mm")
                );
                return newMessage;
            }
            catch (Exception e)
            {
                logService.CreateLog($"Error in SendMessage: {e.Message}");
                throw;
            }
        }

        public async Task<List<MessageDTO>> GetAllMessage(int channelId)
        {
            try
            {
                using var conn = _dbService.CreateConnection();
                await conn.OpenAsync();

                const string query = @"
                    SELECT
                        m.message_id,
                        m.message_content,
                        m.created_at,
                        a.user_name
                    FROM dbo.Message m
                    LEFT JOIN dbo.Account a ON a.user_id = m.id_author
                    WHERE m.id_channel_author = @channelId
                    ORDER BY m.created_at ASC, m.message_id ASC;";

                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@channelId", channelId);

                using var reader = await cmd.ExecuteReaderAsync();
                var history = new List<MessageDTO>();
                while (await reader.ReadAsync())
                {
                    var id = reader.GetInt32(0);
                    var text = reader.IsDBNull(1) ? "" : reader.GetString(1);
                    var createdAt = reader.IsDBNull(2) ? DateTime.UtcNow : reader.GetDateTime(2);
                    var username = reader.IsDBNull(3) ? "Unknown user" : reader.GetString(3);

                    var parisTz = TimeZoneInfo.FindSystemTimeZoneById("Europe/Paris");
                    var parisTime = TimeZoneInfo.ConvertTimeFromUtc(
                        DateTime.SpecifyKind(createdAt, DateTimeKind.Utc),
                        parisTz
                    );
                    history.Add(new MessageDTO(username, id, text, parisTime.ToString("dd/MM HH:mm"))
                    {
                        Username = username,
                        Id = id,
                        Content = text,
                        CreatedAt = parisTime.ToString("dd/MM HH:mm")
                    });
                }

                return history;
            }
            catch (Exception e)
            {
                logService.AddLog(1,$"Error in GetAllMessage: {e.Message}");
                throw;
            }

        }

        public async Task DeleteMessage(int messageId, int channelId)
        {
            try
            {
                using var conn = _dbService.CreateConnection();
                await conn.OpenAsync();

                const string query =
                    @"DELETE FROM Message WHERE message_id = @message_id AND id_channel_author = @channel_id";

                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@message_id", messageId);
                cmd.Parameters.AddWithValue("@channel_id", channelId);
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception e)
            {
                logService.AddLog(1,$"Error in DeleteMessage: {e.Message}");
                throw;
            }
        }

        public async Task<MessageDTO> UpdateMessage(int messageId, int channelId, string newContent)
        {
            try
            {
                using var conn = _dbService.CreateConnection();
                await conn.OpenAsync();

                const string query = @"
                    UPDATE dbo.Message 
                    SET message_content = @newContent
                    WHERE message_id = @message_id AND id_channel_author = @channel_id;

                    SELECT 
                        m.message_id,
                        m.message_content,
                        m.created_at,
                        a.user_name
                    FROM dbo.Message m
                    LEFT JOIN dbo.Account a ON a.user_id = m.id_author
                    WHERE m.message_id = @message_id;";

                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@message_id", messageId);
                cmd.Parameters.AddWithValue("@channel_id", channelId);
                cmd.Parameters.AddWithValue("@newContent", newContent);

                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    var id = reader.GetInt32(0);
                    var content = reader.IsDBNull(1) ? "" : reader.GetString(1);
                    var createdAt = reader.IsDBNull(2) ? DateTime.UtcNow : reader.GetDateTime(2);
                    var username = reader.IsDBNull(3) ? "Unknown user" : reader.GetString(3);

                    var parisTz = TimeZoneInfo.FindSystemTimeZoneById("Europe/Paris");
                    var parisTime = TimeZoneInfo.ConvertTimeFromUtc(
                        DateTime.SpecifyKind(createdAt, DateTimeKind.Utc),
                        parisTz
                    );

                    return new MessageDTO(username, id, content, parisTime.ToString("dd/MM HH:mm"));
                }

                throw new Exception("Message not found or update failed");
            }
            catch (Exception e)
            {
                logService.AddLog(1, $"Error in UpdateMessage: {e.Message}");
                throw;
            }
        }
    }
}

