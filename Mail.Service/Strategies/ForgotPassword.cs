using Mail.Service.Commons.Helpers;
using Mail.Service.Commons.Interface;
using Mail.Service.Enums;
using Mail.Service.Models;
using Mail.Service.Models.Payloads;

namespace Mail.Service.Strategies
{
    /// <summary>
    /// Constrói o e-mail de recuperação de senha.
    /// </summary>
    public sealed class ForgotPassword : IEmailStrategy
    {
        private readonly TemplateHelper _template;

        public ForgotPassword(TemplateHelper template)
        {
            _template = template;
        }

        /// <summary>
        /// Obtém o tipo de e-mail suportado pela estratégia.
        /// </summary>
        public EmailType Type
        {
            get { return EmailType.ForgotPassword; }
        }

        /// <summary>
        /// Gera o conteúdo HTML do e-mail de recuperação de senha.
        /// </summary>
        /// <param name="message">Dados da mensagem de e-mail.</param>
        /// <returns>Assunto e corpo formatado do e-mail.</returns>
        public (string Subject, string HtmlContent) Build(Email message)
        {
            var payload = PayloadHelper.Deserialize<ForgotPasswordPayload>(message.Payload);

            // Título curto e direto
            var subject = "Redefina sua senha";

            var body = $@"
                <h2>Olá, {message.FullName}!</h2>
                <p>Recebemos uma solicitação para redefinir sua senha.</p>
                <p>Utilize o código abaixo para continuar o processo de recuperação.</p>
            
                <div style=""text-align:center;margin:28px 0;"">
                    <div style=""display:inline-block;background:#7C3AED;color:#fff;padding:14px 26px;border-radius:8px;
                                 font-size:22px;letter-spacing:4px;font-weight:600;"">
                        {payload.Code}
                    </div>
                </div>
            
                <p style=""font-size:14px;color:#555;"">
                    Este código expira em alguns minutos. Caso você não tenha solicitado a redefinição, ignore este e-mail com segurança.
                </p>
            ";

            var html = _template.Wrap(body);
            return (subject, html);
        }
    }
}
