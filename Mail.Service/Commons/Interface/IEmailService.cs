using Mail.Service.Models;

namespace Mail.Service.Commons.Interface
{
    /// <summary>
    /// Define o contrato para envio de e-mails.
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Envia um e-mail com base nas informações recebidas.
        /// </summary>
        /// <param name="emailMessage">Mensagem contendo os dados do e-mail.</param>
        Task SendEmail(Email emailMessage);
    }
}
