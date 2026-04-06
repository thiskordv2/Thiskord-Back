using Thiskord_Back.Models.Account;
using Thiskord_Back.Models.Channel;

namespace Thiskord_Back.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Content { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public string Username { get; set; }
        public int UserId { get; set; }
        public int ChannelId { get; set; }

        public Channel.Channel Channel { get; set; } = null!;
        
        public Message(string username, int idMessage, string content, DateTime createdAt)
        {
            Username = username;
            Id = idMessage;
            Content = content;
            CreatedAt = createdAt;
        }
    }
    
    
}