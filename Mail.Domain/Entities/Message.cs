using System.Text.Json.Serialization;

namespace Mail.Domain.Entities
{
    public sealed record class Message
    {
        [JsonPropertyName("id")]
        public Guid Id { get; init; }

        [JsonPropertyName("type")]
        public Enums.Type Type { get; init; }

        [JsonPropertyName("to")]
        public string To { get; init; } = string.Empty;

        [JsonPropertyName("full_name")]
        public string FullName { get; init; } = string.Empty;

        [JsonPropertyName("content")]
        public string Content { get; init; } = string.Empty;

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    }
}
