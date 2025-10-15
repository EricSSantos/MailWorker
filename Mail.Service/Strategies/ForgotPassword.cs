using Mail.Service.Commons.Helpers;
using Mail.Service.Commons.Interface;
using Mail.Service.Enums;
using Mail.Service.Models;
using Mail.Service.Models.Payloads;
using System.Text.Json;

namespace Mail.Service.Strategies
{
    /// <summary>
    /// Estratégia para geração de e-mail de recuperação de senha (OTP).
    /// </summary>
    public sealed class ForgotPassword : IEmailStrategy
    {
        public EmailType Type
        {
            get { return EmailType.ForgotPassword; }
        }

        public (string Subject, string HtmlContent) Build(Email message)
        {
            var payload = message.Payload.HasValue
                ? JsonSerializer.Deserialize<ForgotPasswordPayload>(message.Payload.Value)
                : null;

            if (payload is null || string.IsNullOrWhiteSpace(payload.Code))
                throw new InvalidOperationException("Payload inválido para e-mail de recuperação de senha.");

            var subject = "Recuperação de senha";

            var body = $@"
                <h2>Olá, {message.FullName}!</h2>
                <p>Você solicitou uma recuperação de senha.</p>
                <p>Use o código abaixo para continuar o processo.</p>
            
                <div style=""text-align: center;"">
                    <strong style=""font-size: 24px;"">{payload.Code}</strong>
                </div>
            
                <p>Se você não solicitou esta ação, apenas ignore este e-mail.</p>
            ";

            var html = Template.Wrap(body);

            return (subject, html);
        }
    }
}
