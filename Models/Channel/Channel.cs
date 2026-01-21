namespace Thiskord_Back.Models.Channel
{
    public class Channel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class ChannelRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
    public class ChannelUser
    {
        public int Id { get; set; }
        public int ChannelId { get; set; }
        public string UserId { get; set; }
    }
}
