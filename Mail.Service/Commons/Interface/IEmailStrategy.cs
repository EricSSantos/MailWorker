using Mail.Service.Enums;
using Mail.Service.Models;

namespace Mail.Service.Commons.Interface
{
    /// <summary>
    /// Define o contrato para construção de e-mails transacionais.
    /// </summary>
    public interface IEmailStrategy
    {
        /// <summary>
        /// Obtém o tipo de e-mail suportado pela estratégia.
        /// </summary>
        EmailType Type { get; }

        /// <summary>
        /// Gera o assunto e o conteúdo HTML do e-mail.
        /// </summary>
        /// <param name="message">Mensagem contendo os dados do e-mail.</param>
        /// <returns>Assunto e corpo formatado do e-mail.</returns>
        (string Subject, string HtmlContent) Build(Email message);
    }
}
