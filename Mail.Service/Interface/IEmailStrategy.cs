using Mail.Domain.Entities;
using Mail.Domain.Enums;

namespace Mail.Service.Interface
{
    public interface IEmailStrategy
    {
        Domain.Enums.Type Type { get; }
        (string Subject, string HtmlContent) Build(Message message);
    }
}
