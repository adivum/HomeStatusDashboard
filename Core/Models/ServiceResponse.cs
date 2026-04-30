using System.Text.Json.Serialization;

namespace Core.Models
{
    public class ServiceResponse
    {
        [JsonPropertyName("context")]
        public Context? Context { get; set; }
    }

    public class Context
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("parent_id")]
        public string? ParentId { get; set; }

        [JsonPropertyName("user_id")]
        public string? UserId { get; set; }
    }
}
