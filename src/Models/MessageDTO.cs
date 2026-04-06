namespace Thiskord_Back.Models
{
    public class MessageDTO
    {
        public int Id { get; set; }
        public string Content { get; set; } = null!;
        public string CreatedAt { get; set; }
        public string Username { get; set; }
    
        public MessageDTO(string username, int idMessage, string content, string createdAt)
        {
            Username = username;
            Id = idMessage;
            Content = content;
            CreatedAt = createdAt;
        }
    }
}

