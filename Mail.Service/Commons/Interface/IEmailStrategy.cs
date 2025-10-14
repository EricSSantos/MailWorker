using Mail.Service.Enums;
using Mail.Service.Models;

namespace Mail.Service.Commons.Interface
{
    public interface IEmailStrategy
    {
        EmailType Type { get; }
        (string Subject, string HtmlContent) Build(Message message);
    }
}
