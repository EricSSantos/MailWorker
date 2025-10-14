using Mail.Service.Commons.Interface;
using Mail.Service.Commons.Settings;
using Mail.Service.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Mail.Service
{
    public sealed class EmailService : IEmailService
    {
        private readonly SmtpSettings _smtp;
        private readonly ILogger<EmailService> _logger;
        private readonly IEnumerable<IEmailStrategy> _strategies;

        public EmailService(
            IOptions<SmtpSettings> smtp,
            ILogger<EmailService> logger,
            IEnumerable<IEmailStrategy> strategies)
        {
            _smtp = smtp.Value;
            _logger = logger;
            _strategies = strategies;
        }

        public async Task SendEmail(Email emailMessage)
        {
            if (string.IsNullOrWhiteSpace(emailMessage.To))
            {
                _logger.LogWarning(
                    "Mensagem ignorada: destinatário ausente. Id={Id}, Tipo={Type}",
                    emailMessage.Id,
                    emailMessage.Type
                );

                return;
            }

            var strategy = _strategies.FirstOrDefault(s => s.Type == emailMessage.Type);
            if (strategy is null)
            {
                _logger.LogError(
                    "Nenhuma estratégia encontrada para o tipo {Type}. Id={Id}",
                    emailMessage.Type,
                    emailMessage.Id);

                throw new InvalidOperationException(
                    $"Nenhuma estratégia configurada para o tipo {emailMessage.Type}. Id={emailMessage.Id}"
                );
            }

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
                    "E-mail enviado com sucesso. Id={Id}, Tipo={Type}, Hora={Time}",
                    emailMessage.Id,
                    emailMessage.Type,
                    DateTime.UtcNow.ToString("O")
                );
            }
            catch (SmtpException smtpEx)
            {
                _logger.LogError(
                    smtpEx,
                    "Erro SMTP ao enviar e-mail. Id={Id}, Tipo={Type}, Status={Status}, Hora={Time}",
                    emailMessage.Id,
                    emailMessage.Type,
                    smtpEx.StatusCode,
                    DateTime.UtcNow.ToString("O")
                );
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Erro inesperado ao enviar e-mail. Id={Id}, Tipo={Type}, Hora={Time}",
                    emailMessage.Id,
                    emailMessage.Type,
                    DateTime.UtcNow.ToString("O")
                );
                throw;
            }
        }
    }
}
