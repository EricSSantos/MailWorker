using Mail.Service.Enums;
using System.Text.Json.Serialization;

namespace Mail.Service.Models
{
    public sealed class Message
    {
        #region Properties

        [JsonPropertyName("to")]
        public string To { get; private set; }

        [JsonPropertyName("full_name")]
        public string FullName { get; private set; }

        [JsonPropertyName("type")]
        public EmailType Type { get; private set; }

        [JsonPropertyName("content")]
        public string Content { get; private set; }

        [JsonPropertyName("created_at")]
        public DateTimeOffset CreatedAt { get; private set; }

        #endregion

        #region Constructors

        protected Message()
        {
            // necessário para deserialização via System.Text.Json
            To = string.Empty;
            FullName = string.Empty;
            Content = string.Empty;
            CreatedAt = DateTimeOffset.UtcNow;
        }

        private Message(string to, string fullName, EmailType type, string content, DateTimeOffset createdAt)
        {
            To = to;
            FullName = fullName;
            Type = type;
            Content = content;
            CreatedAt = createdAt;
        }

        #endregion

        #region Factory

        public static Message Create(string to, string fullName, EmailType type, string content)
        {
            if (string.IsNullOrWhiteSpace(to))
                throw new ArgumentException("O email do destinatário não pode ser vazio.");

            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("O nome destinatário não pode ser vazio.");

            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("O conteúdo do e-mail não pode ser vazio.");

            return new Message(
                to: to.Trim(),
                fullName: fullName.Trim(),
                type: type,
                content: content,
                createdAt: DateTimeOffset.UtcNow
            );
        }

        #endregion
    }
}
