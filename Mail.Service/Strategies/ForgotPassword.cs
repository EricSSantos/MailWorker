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
        private const string SUBJECT = "Recuperação de senha";

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

            var body = $@"
                <h2>Olá, {message.FullName}!</h2>
                <p>Recebemos uma solicitação para redefinir sua senha.</p>
                <p>Use o código abaixo para continuar o processo de recuperação:</p>
            
                <div style=""text-align: center; margin: 24px 0;"">
                    <strong style=""font-size: 26px; letter-spacing: 4px;"">{payload.Code}</strong>
                </div>
            
                <p>Por segurança, este código expira em poucos minutos.</p>
                <p>Se você não fez esta solicitação, pode ignorar este e-mail com tranquilidade.</p>
            ";

            return (SUBJECT, TemplateHelper.Wrap(body));
        }
    }
}
