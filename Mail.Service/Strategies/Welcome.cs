using Mail.Service.Commons.Interface;
using Mail.Service.Enums;
using Mail.Service.Models;

namespace Mail.Service.Strategies
{
    public sealed class Welcome : IEmailStrategy
    {
        public EmailType Type
        {
            get { return EmailType.Welcome; }
        }

        public (string Subject, string HtmlContent) Build(Message message)
        {
            var subject = "Boas-vindas!";

            var html = $@"
                <h2>Olá, {message.FullName.Trim()}!</h2>
                <p>Estamos muito felizes em ter você conosco.</p>
                <p>Agora você pode aproveitar todos os recursos da nossa plataforma.</p>
                <br/>
            ";

            return (subject, html);
        }
    }
}
