using Mail.Service.Commons.Helpers;
using Mail.Service.Commons.Interface;
using Mail.Service.Enums;
using Mail.Service.Models;
using Mail.Service.Models.Payloads;

namespace Mail.Service.Strategies
{
    /// <summary>
    /// Gera o e-mail de confirmação de conta.
    /// </summary>
    public sealed class ConfirmEmail : IEmailStrategy
    {
        private readonly TemplateHelper _template;

        public ConfirmEmail(TemplateHelper template)
        {
            _template = template;
        }

        /// <summary>
        /// Obtém o tipo de e-mail manipulado por esta estratégia.
        /// </summary>
        public EmailType Type
        {
            get { return EmailType.ConfirmEmail; }
        }

        /// <summary>
        /// Constrói o assunto e o conteúdo HTML do e-mail de confirmação.
        /// </summary>
        /// <param name="message">Dados da mensagem a ser processada.</param>
        /// <returns>Tupla contendo o assunto e o corpo HTML formatado do e-mail.</returns>
        public (string Subject, string HtmlContent) Build(Email message)
        {
            var payload = PayloadHelper.Deserialize<ConfirmEmailPayload>(message.Payload);
            var subject = $"Confirme seu e-mail";

            var body = $@"
                <h2>Olá, {message.FullName}!</h2>
                <p>Estamos quase lá! Para ativar sua conta, utilize o código abaixo.</p>

                <div style=""text-align:center;margin:28px 0;"">
                    <div style=""display:inline-block;background:#7C3AED;color:#fff;padding:14px 26px;border-radius:8px;
                                 font-size:22px;letter-spacing:4px;font-weight:600;"">
                        {payload.Code}
                    </div>
                </div>

                <p style=""font-size:14px;color:#555;"">
                    Este código expira em alguns minutos. Caso não tenha solicitado o cadastro, ignore este e-mail com segurança.
                </p>
            ";

            var html = _template.Wrap(body);
            return (subject, html);
        }
    }
}
