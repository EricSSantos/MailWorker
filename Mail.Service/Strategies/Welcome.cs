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

        /// <summary>
        /// Inicializa a estratégia com as configurações da aplicação.
        /// </summary>
        /// <param name="options">Configurações da aplicação.</param>
        public Welcome(IOptions<ApplicationSettings> options)
        {
            _app = options.Value;
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
            var subject = $"Bem-vindo ao {_app.Name}!";

            var body = $@"
                <h2>Olá, {message.FullName}!</h2>
                <p>Estamos muito felizes em ter você conosco.</p>
                <p>Agora você pode aproveitar todos os recursos da nossa plataforma {_app.Name}.</p>
                <p>Atenciosamente, equipe {_app.Name}.</p>
            ";

            var html = TemplateHelper.Wrap(body);
            return (subject, html);
        }
    }
}
