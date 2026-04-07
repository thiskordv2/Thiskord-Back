namespace Thiskord_Back.Models
{
    public class Invitation
    {
        public int id { get; set; }
        public string token { get; set; }
        public int projectId { get; set; }
        public int creatorId { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime? expiresAt { get; set; } = null;
    }    
    public record CreateInviteRequest(int projectId, int creatorId, DateTime? expiresAt);
}

