using System.Text.Json.Serialization;

namespace Core.Models
{
    public class HaState
    {
        [JsonPropertyName("entity_id")]
        public string EntityId { get; set; } = string.Empty;

        [JsonPropertyName("state")]
        public string State { get; set; } = string.Empty;

        [JsonPropertyName("attributes")]
        public Dictionary<string, object?> Attributes { get; set; } = new();

        [JsonPropertyName("last_changed")]
        public DateTime LastChanged { get; set; }

        [JsonPropertyName("last_updated")]
        public DateTime LastUpdated { get; set; }
    }
}