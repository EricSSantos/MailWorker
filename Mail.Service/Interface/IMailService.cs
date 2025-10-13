using Mail.Domain.Entities;

namespace Mail.Service.Interface
{
    public interface IMailService
    {
        /// <summary>
        /// Envia um e-mail com base nas informações fornecidas.
        /// </summary>
        Task SendEmail(Message emailMessage);
    }
}
