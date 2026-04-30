using System.Text.Json.Serialization;

namespace Core.Models
{
    public class ServiceRequest
    {
        [JsonPropertyName("entity_id")]
        public string EntityId { get; set; } = string.Empty;
    }
}
