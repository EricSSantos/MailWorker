using Mail.Service.Commons.Interface;
using Mail.Service.Commons.Settings;
using Mail.Service.Models;
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
            var strategy = _strategies.FirstOrDefault(s => s.Type == emailMessage.Type);

            if (strategy is null)
                throw new InvalidOperationException($"Nenhuma estratégia encontrada para o tipo {emailMessage.Type}.");

            var (subject, htmlContent) = strategy.Build(emailMessage);

            try
            {
                using var smtpClient = new SmtpClient(_smtp.Host)
                {
                    Port = _smtp.Port,
                    Credentials = new NetworkCredential(_smtp.User, _smtp.Password),
                    EnableSsl = _smtp.EnableSsl
                };

                using var message = new MailMessage
                {
                    From = new MailAddress(_smtp.FromEmail, _smtp.FromName),
                    Subject = subject,
                    Body = htmlContent,
                    IsBodyHtml = true
                };

                message.To.Add(new MailAddress(emailMessage.To));

                await smtpClient.SendMailAsync(message);

                _logger.LogInformation(
                    "E-mail {Type} enviado com sucesso para {Email}",
                    emailMessage.Type,
                    emailMessage.To
                );
            }
            catch (SmtpException smtpEx)
            {
                _logger.LogError(
                    smtpEx,
                    "Erro SMTP ao enviar e-mail para {Email}: {Message}",
                    emailMessage.To,
                    smtpEx.Message
                );
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Erro inesperado ao enviar e-mail para {Email}",
                    emailMessage.To
                );
                throw;
            }
        }
    }
}
