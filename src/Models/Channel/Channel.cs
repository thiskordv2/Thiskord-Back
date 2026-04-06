using System.Text.Json.Serialization;

namespace Thiskord_Back.Models.Channel
{
    public class Channel
    {
        public int? id { get; set; }
        public string? name { get; set; }
        public string? description { get; set; }
        
        public Channel() {}
    }

    public class ChannelRequest
    {
        [JsonPropertyName("name")]
        public string name { get; set; }

        [JsonPropertyName("description")]
        public string description { get; set; }
    
        [JsonPropertyName("projectId")]
        public int projectId { get; set; }
    }
}
