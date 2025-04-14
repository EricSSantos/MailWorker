using Mail.Domain.Entities;

namespace Mail.Service.Interface
{
    public interface IMailService
    {
        Task SendEmail(EmailMessage emailMessage);
    }
}
