using Mail.Domain.Enums;
using System.Text.Json;

namespace Mail.Domain.Entities
{
    /// <summary>
    /// Representa uma mensagem de e-mail, contendo informações sobre o destinatário, tipo e conteúdo.
    /// </summary>
    public class EmailMessage
    {
        /// <summary>
        /// Identificador único da mensagem de e-mail.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Tipo da mensagem de e-mail (ex: confirmação de cadastro, recuperação de senha, etc.).
        /// </summary>
        public EmailType Type { get; set; }

        /// <summary>
        /// Endereço de e-mail do destinatário.
        /// </summary>
        public string To { get; set; } = string.Empty;

        /// <summary>
        /// Nome completo do destinatário.
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Conteúdo da mensagem de e-mail em formato JSON.
        /// </summary>
        public JsonElement Content { get; set; }

        /// <summary>
        /// Data e hora em que a mensagem de e-mail foi criada.
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
