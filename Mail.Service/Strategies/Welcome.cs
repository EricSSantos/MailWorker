using Mail.Service.Commons.Helpers;
using Mail.Service.Commons.Interface;
using Mail.Service.Commons.Settings;
using Mail.Service.Enums;
using Mail.Service.Models;
using Microsoft.Extensions.Options;

namespace Mail.Service.Strategies
{
    /// <summary>
    /// Constrói o e-mail de boas-vindas ao usuário.
    /// </summary>
    public sealed class Welcome : IEmailStrategy
    {
        private readonly ApplicationSettings _app;
        private readonly TemplateHelper _template;

        public Welcome(IOptions<ApplicationSettings> options, TemplateHelper template)
        {
            _app = options.Value;
            _template = template;
        }

        /// <summary>
        /// Obtém o tipo de e-mail suportado pela estratégia.
        /// </summary>
        public EmailType Type
        {
            get { return EmailType.Welcome; }
        }

        /// <summary>
        /// Gera o conteúdo HTML do e-mail de boas-vindas.
        /// </summary>
        /// <param name="message">Dados da mensagem de e-mail.</param>
        /// <returns>Assunto e corpo formatado do e-mail.</returns>
        public (string Subject, string HtmlContent) Build(Email message)
        {
            var subject = $"Boas-vindas ao {_app.Name}!";

            var body = $@"
                <h2>Olá, {message.FullName}!</h2>
                <p>Boas-vindas ao <strong>{_app.Name}</strong>!</p>
                <p>Agora você faz parte da nossa comunidade e pode aproveitar todos os recursos disponíveis.</p>

                <div style=""text-align:center;margin:30px 0;"">
                    <div style=""display:inline-block;background:#7C3AED;color:#fff;
                                padding:12px 24px;border-radius:6px;font-size:15px;font-weight:600;"">
                        Sua jornada começa agora 🚀
                    </div>
                </div>
            ";

            var html = _template.Wrap(body);
            return (subject, html);
        }
    }
}
