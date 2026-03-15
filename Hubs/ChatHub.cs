using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Thiskord_Back.Services;

namespace Thiskord_Back.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IDbConnectionService _dbService;
        
        public ChatHub(IDbConnectionService dbService)
        {
            _dbService = dbService;
        }
        
        public async Task JoinChannel(int channelId)
        {
            // Connection de l'utilisateur au channel via l'id
            await Groups.AddToGroupAsync(Context.ConnectionId, channelId.ToString());
            
            // Récupération de l'historique des messages du channel
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
            
            // Execution de la requête
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@channelId", channelId);
            
            // Lecture de tous les messages
            using var reader = await cmd.ExecuteReaderAsync();
            var history = new List<object>();
            // Ajout de tous les messages à un tableau avec le text, l'auteur et la date de création
            while (await reader.ReadAsync())
            {
                var text = reader.IsDBNull(1) ? "" : reader.GetString(1);
                var createdAt = reader.IsDBNull(2) ? DateTime.UtcNow : reader.GetDateTime(2);
                var username = reader.IsDBNull(3) ? "Unknown user" : reader.GetString(3);

                var parisTz = TimeZoneInfo.FindSystemTimeZoneById("Europe/Paris");
                var parisTime = TimeZoneInfo.ConvertTimeFromUtc(
                    DateTime.SpecifyKind(createdAt, DateTimeKind.Utc),
                    parisTz
                );

                history.Add(new
                {
                    user = username,
                    text,
                    dateTime = parisTime.ToString("dd/MM HH:mm")
                });
            }
            // Envoie de l'historique de message
            await Clients.Caller.SendAsync("LoadMessages", history);
            
            // OPT : Message pour prévenir que quelqu'un vient de rejoindre
            await Clients.Group(channelId.ToString()).SendAsync("UserJoined", Context.User?.Identity?.Name ?? "Unknown user", $"has joined the channel {channelId}");
        }

        public async Task LeaveChannel(int channelId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, channelId.ToString());
        }
        
        public async Task SendMessage(int channelId, string text)
        {
            var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
                throw new HubException("Utilisateur non authentifie");

            using var conn = _dbService.CreateConnection();
            await conn.OpenAsync();

            const string query = @"
            INSERT INTO dbo.Message (id_channel_author, id_author, message_content)
            VALUES (@channelId, @userId, @text);";

            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@channelId", channelId);
            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.Parameters.AddWithValue("@text", text);
            await cmd.ExecuteNonQueryAsync();

            var username = Context.User?.Identity?.Name ?? $"user#{userId}";
            var parisTz = TimeZoneInfo.FindSystemTimeZoneById("Europe/Paris");
            var dateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, parisTz).ToString("dd/MM HH:mm");

            await Clients.Group(channelId.ToString())
                .SendAsync("ReceiveMessage", username, text, dateTime);
        }
    }
}
