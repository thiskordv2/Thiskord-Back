namespace Thiskord_Back.Models.Channel
{
    public class Channel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
    }

    public class ChannelRequest
    {
        public string name { get; set; }
        public string description { get; set; }
    }
    public class ChannelUser
    {
        public int id { get; set; }
        public int channelId { get; set; }
        public string userId { get; set; }
    }
}
