using System.Text.Json;
using System.Text.Json.Serialization;

namespace Thiskord_Back.Models.Project
{
    public class Project
    {
        public int? id { get; set; }
        public string? name { get; set; }
        public string? description { get; set; }

        public Project(int id, string name, string description)
        {
            this.id= id;
            this.name = name;
            this.description = description;
        }

        public class ProjectRequest
        {
            [JsonPropertyName("project_name")]
            public string name { get; set; }
            
            [JsonPropertyName("project_desc")]
            public string description { get; set; }
        }
    }
    

}
