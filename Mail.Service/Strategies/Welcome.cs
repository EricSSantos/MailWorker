using Mail.Service.Commons.Helpers;
using Mail.Service.Commons.Interface;
using Mail.Service.Commons.Settings;
using Mail.Service.Enums;
using Mail.Service.Models;
using Microsoft.Extensions.Options;

namespace Mail.Service.Strategies
{
    public sealed class Welcome : IEmailStrategy
    {
        private readonly ApplicationSettings _app;

        public Welcome(IOptions<ApplicationSettings> options)
        {
            _app = options.Value;
        }

        public EmailType Type
        {
            get { return EmailType.Welcome; }
        }

        public (string Subject, string HtmlContent) Build(Email message)
        {
            var subject = $"Bem-vindo ao {_app.Name}!";

            var body = $@"
                <h2>Olá, {message.FullName.Trim()}!</h2>
                <p>Estamos muito felizes em ter você conosco.</p>
                <p>Agora você pode aproveitar todos os recursos da nossa plataforma {_app.Name}.</p>
                <p>Atenciosamente,<br/>Equipe {_app.Name}</p>
            ";

            var html = Template.Wrap(body);
            return (subject, html);
        }
    }
}
