using Mail.Service.Enums;
using Mail.Service.Models;

namespace Mail.Service.Commons.Interface
{
    public interface IEmailStrategy
    {
        /// <summary>
        /// Tipo de e-mail que a estratégia representa.
        /// Utilizado para identificar automaticamente
        /// qual implementação deve ser aplicada.
        /// </summary>
        EmailType Type { get; }

        /// <summary>
        /// Constrói o assunto e o corpo HTML do e-mail
        /// com base nas informações fornecidas na mensagem.
        /// </summary>
        /// <param name="message">Mensagem contendo os dados do destinatário e conteúdo dinâmico.</param>
        /// <returns>Uma tupla contendo o assunto e o conteúdo HTML do e-mail.</returns>
        (string Subject, string HtmlContent) Build(Email message);
    }
}
