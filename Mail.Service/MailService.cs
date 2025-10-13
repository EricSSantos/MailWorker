using Mail.Domain.Entities;
using Mail.Service.Helpers.Settings;
using Mail.Service.Interface;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Mail.Service
{
    public sealed class MailService : IMailService
    {
        private readonly SmtpSettings _smtp;
        private readonly ILogger<MailService> _logger;
        private readonly IEnumerable<IEmailStrategy> _strategies;

        public MailService(
            IOptions<SmtpSettings> smtp,
            ILogger<MailService> logger,
            IEnumerable<IEmailStrategy> strategies)
        {
            _smtp = smtp.Value;
            _logger = logger;
            _strategies = strategies;
        }

        public async Task SendEmail(Message emailMessage)
        {
            try
            {
                var strategy = _strategies.FirstOrDefault(s => s.Type == emailMessage.Type)
                    ?? throw new ArgumentException($"Nenhuma estratégia configurada para o tipo {emailMessage.Type}");

                var (subject, htmlContent) = strategy.Build(emailMessage);

                using var smtpClient = new SmtpClient(_smtp.Host)
                {
                    Port = _smtp.Port,
                    Credentials = new NetworkCredential(_smtp.User, _smtp.Password),
                    EnableSsl = _smtp.EnableSsl
                };

                using var message = new MailMessage(
                    new MailAddress(_smtp.FromEmail, _smtp.FromName),
                    new MailAddress(emailMessage.To))
                {
                    Subject = subject,
                    Body = htmlContent,
                    IsBodyHtml = true
                };

                await smtpClient.SendMailAsync(message);
                _logger.LogInformation("E-mail enviado com sucesso para {Email}", emailMessage.To);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar e-mail para {Email}", emailMessage.To);
                throw;
            }
        }
    }
}
