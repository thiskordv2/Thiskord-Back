using Microsoft.AspNetCore.SignalR;

namespace Thiskord_Back.Hubs
{
    public class ChatHub : Hub
    {
        
        public async Task JoinChannel(int channelId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, channelId.ToString());
        }

        public async Task LeaveChannel(int channelId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, channelId.ToString());
        }
        
        public async Task SendMessage(int channelId, string text)
        {
            var username = Context.GetHttpContext()?.Request.Headers["username"].ToString();
            
            var parisTz = TimeZoneInfo.FindSystemTimeZoneById("Europe/Paris");
            var parisNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, parisTz);
            var dateTime = parisNow.ToString("HH:mm");

            await Clients.Group(channelId.ToString())
                .SendAsync("ReceiveMessage", username, text, dateTime);
        }
    }
}
