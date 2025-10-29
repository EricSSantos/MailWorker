using Mail.Service.Enums;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Mail.Service.Models
{
    /// <summary>
    /// Representa um e-mail transacional gerado pela aplicação.
    /// </summary>
    public sealed class Email
    {
        /// <summary>
        /// Identificador único do e-mail.
        /// </summary>
        [JsonPropertyName("id")]
        public Guid Id { get; init; }

        /// <summary>
        /// Endereço de destino do e-mail.
        /// </summary>
        [JsonPropertyName("to")]
        public string To { get; init; } = string.Empty;

        /// <summary>
        /// Nome completo do destinatário.
        /// </summary>
        [JsonPropertyName("full_name")]
        public string FullName { get; init; } = string.Empty;

        /// <summary>
        /// Tipo de e-mail transacional.
        /// </summary>
        [JsonPropertyName("type")]
        public EmailType Type { get; init; }

        /// <summary>
        /// Dados adicionais do e-mail serializados em JSON.
        /// </summary>
        [JsonPropertyName("payload")]
        public JsonElement? Payload { get; init; }

        /// <summary>
        /// Data e hora de criação do e-mail.
        /// </summary>
        [JsonPropertyName("created_at")]
        public DateTimeOffset CreatedAt { get; init; }
    }
}
