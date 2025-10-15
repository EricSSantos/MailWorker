using Mail.Service.Enums;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Mail.Service.Models
{
    public sealed class Email
    {
        [JsonPropertyName("id")]
        public Guid Id { get; init; }

        [JsonPropertyName("to")]
        public string To { get; init; } = string.Empty;

        [JsonPropertyName("full_name")]
        public string FullName { get; init; } = string.Empty;

        [JsonPropertyName("type")]
        public EmailType Type { get; init; }

        [JsonPropertyName("payload")]
        public JsonElement? Payload { get; init; }

        [JsonPropertyName("created_at")]
        public DateTimeOffset CreatedAt { get; init; }
    }
}
