using System.Text.Json;
using System.Text.Json.Serialization;

namespace Thiskord_Back.Models.Project
{
    public class Project
    {
        public int? id { get; set; }
        public string? name { get; set; }
        public string? description { get; set; }
        
    }
    public class ProjectRequest
    {
        [JsonPropertyName("name")]
        public string name { get; set; }
            
        [JsonPropertyName("description")]
        public string description { get; set; }
    }
    
    

}
