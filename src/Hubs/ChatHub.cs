using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Thiskord_Back.Services;

namespace Thiskord_Back.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IMessageService _messageService;
        
        public ChatHub(IMessageService messageService)
        {
            _messageService = messageService;
        }
        
        public async Task JoinChannel(int channelId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, channelId.ToString());
            var historique = await _messageService.GetAllMessage(channelId);
            await Clients.Caller.SendAsync("LoadMessages", historique);
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
            var username = Context.User?.Identity?.Name ?? $"user#{userId}";

            var response = await _messageService.SendMessage(channelId, userId, text, username);
            await Clients.Group(channelId.ToString())
                .SendAsync("ReceiveMessage", response.Id, response.Username, response.Content, response.CreatedAt);
        }

        public async Task DeleteMessage(int channelId, int messageId)
        {
            await _messageService.DeleteMessage(messageId, channelId);
            await Clients.Group(channelId.ToString())
                .SendAsync("DeleteMessage", messageId);
        }

        public async Task EditMessage(int channelId, int messageId, string newContent)
        {
            try
            {
                var updatedMessage = await _messageService.UpdateMessage(messageId, channelId, newContent);
                await Clients.Group(channelId.ToString())
                    .SendAsync("EditMessage", updatedMessage.Id, updatedMessage.Content, updatedMessage.CreatedAt);
            }
            catch (HubException ex)
            {
                await Clients.Caller.SendAsync("Error", ex.Message);
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Error", "Erreur lors de l'édition du message");
                throw;
            }
        }
    }
}
