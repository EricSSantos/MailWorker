using Mail.Service.Models;

namespace Mail.Service.Commons.Interface
{
    public interface IMailService
    {
        /// <summary>
        /// Envia um e-mail com base nas informações fornecidas.
        /// </summary>
        /// <param name="emailMessage">Mensagem recebida da fila contendo as informações do e-mail.</param>
        Task SendEmail(Message emailMessage);
    }
}
