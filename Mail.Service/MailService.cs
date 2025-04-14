using Mail.Domain.Entities;
using Mail.Domain.Enums;
using Mail.Service.Interface;
using Mail.Service.Utils;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Text.Json;

namespace Mail.Service
{
    public class MailService : IMailService
    {
        #region Dependencies
        private readonly string _key;
        private readonly string _fromEmail;
        private readonly string _fromName;
        private readonly string _resetPasswordUrl;

        public MailService(IConfiguration configuration)
        {
            var sendGridSection = configuration.GetSection("SendGrid");
            var applicationSection = configuration.GetSection("Application");

            _key = sendGridSection["Key"]!;
            _fromEmail = sendGridSection["FromEmail"]!;
            _fromName = sendGridSection["FromName"]!;

            _resetPasswordUrl = applicationSection["ResetPasswordUrl"]!;
        }
        #endregion

        /// <summary>
        /// Envia um e-mail para o destinatário, com base no tipo de e-mail e conteúdo fornecido.
        /// Realiza a geração do conteúdo e o envio usando o SendGrid.
        /// </summary>
        /// <param name="type">Tipo do e-mail a ser enviado.</param>
        /// <param name="to">Endereço de e-mail do destinatário.</param>
        /// <param name="content">Conteúdo do e-mail em formato Json.</param>
        public async Task SendEmail(EmailMessage emailMessage)
        {
            try
            {
                var client = new SendGridClient(_key);
                var from = new EmailAddress(_fromEmail, _fromName);
                var recipientEmail = new EmailAddress(emailMessage.To);

                var (subject, htmlContent) = Generate(emailMessage.Type, emailMessage.Content, emailMessage.FullName);
                var message = MailHelper.CreateSingleEmail(from, recipientEmail, subject, plainTextContent: null, htmlContent);

                var response = await client.SendEmailAsync(message);

                if (!response.IsSuccessStatusCode)
                {
                    var body = await response.Body.ReadAsStringAsync();
                    throw new InvalidOperationException($"Falha ao enviar e-mail: {response.StatusCode} - {body}");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Falha ao enviar e-mail", ex);
            }
        }

        #region Private Methods
        /// <summary>
        /// Gera o assunto e conteúdo HTML do e-mail com base no tipo e no conteúdo fornecido.
        /// </summary>
        /// <param name="type">Tipo do e-mail a ser gerado.</param>
        /// <param name="content">Conteúdo do e-mail em formato Json.</param>
        /// <returns>Retorna o assunto e o conteúdo HTML do e-mail.</returns>
        private (string Subject, string HtmlContent) Generate(EmailType type, JsonElement content, string fullName)
        {
            switch (type)
            {
                case EmailType.AccountConfirmation:
                    return AccountConfirmationEmail(ContentHelper.DeserializeContent<AccountConfirmation>(content), fullName);

                case EmailType.PasswordReset:
                    return PasswordResetEmail(ContentHelper.DeserializeContent<PasswordReset>(content), fullName);

                default:
                    throw new ArgumentOutOfRangeException(nameof(type), $"Tipo de e-mail não suportado ou conteúdo inválido: {type}");
            }
        }

        /// <summary>
        /// Gera o conteúdo de um e-mail de confirmação de conta, incluindo o nome do usuário e o código de confirmação.
        /// </summary>
        /// <param name="content">Dados de confirmação de conta (nome do usuário e código).</param>
        /// <returns>Retorna o assunto e o conteúdo HTML do e-mail de confirmação de conta.</returns>
        private (string Subject, string HtmlContent) AccountConfirmationEmail(AccountConfirmation content, string fullName)
        {
            const string SUBJECT = "Confirmação de Cadastro";

            var htmlContent = $@"
                <h2 style=""margin: 0; color: #333333; font-size: 22px;"">
                    Seja bem-vindo, <span style=""color: #27ae60;"">{fullName}</span>
                </h2>
                <p>Seu cadastro foi realizado com sucesso.</p>
                <p>Para concluir seu cadastro, utilize o código de confirmação abaixo:</p>
                <div style='font-size: 24px; font-weight: bold; letter-spacing: 4px; background-color: #27ae60; padding: 15px; width: fit-content; border-radius: 8px; margin: 20px auto; text-align: center; color: white;'>
                    {content.Code}
                </div>";

            return (SUBJECT, Wrap(htmlContent));
        }

        /// <summary>
        /// Gera o conteúdo de um e-mail de reset de senha, incluindo o token e a URL para o novo login.
        /// </summary>
        /// <param name="content">Objeto de EmailMessage que contém o nome completo e dados de reset de senha.</param>
        /// <returns>Retorna o assunto e o conteúdo HTML do e-mail de reset de senha.</returns>
        private (string Subject, string HtmlContent) PasswordResetEmail(PasswordReset content, string fullName)
        {
            const string SUBJECT = "Redefinição de Senha";

            var htmlContent = $@"
                <h2 style=""margin: 0; color: #333333; font-size: 22px;"">
                    Olá, <span style=""color: #27ae60;"">{fullName}</span>
                </h2>
                <p>Recebemos uma solicitação para redefinir a senha da sua conta.</p>
                <p>Clique no link abaixo para redefinir sua senha:</p>
                <div style=""font-size: 24px; font-weight: bold; letter-spacing: 4px; background-color: #27ae60; padding: 15px; width: fit-content; border-radius: 8px; margin: 20px auto; text-align: center; color: white;"">
                    <a href=""{_resetPasswordUrl}{content.Token}"" style=""background-color: #27ae60; padding: 10px 20px; color: white; text-decoration: none; border-radius: 5px; font-weight: bold;"">
                        Redefinir Senha
                    </a>
                </div>
                <p>Se você não fez essa solicitação, ignore este e-mail.</p>";


            return (SUBJECT, Wrap(htmlContent));
        }

        /// <summary>
        /// Envolve o conteúdo HTML gerado em uma estrutura completa de e-mail, incluindo cabeçalho, corpo e rodapé.
        /// </summary>
        /// <param name="htmlContent">Conteúdo HTML principal do e-mail.</param>
        /// <returns>Retorna o conteúdo completo do e-mail, com formatação básica.</returns>
        private string Wrap(string htmlContent)
        {
            return $@"
                <!DOCTYPE html>
                <html lang=""pt-br"">
                <head>
                    <meta charset=""UTF-8"">
                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                </head>
                <body style=""margin: 0; padding: 0; font-family: 'Segoe UI';"">
                    <div style=""margin: 40px auto; max-width: 600px; background-color: #f7f7f7; border-radius: 10px; overflow: hidden; box-shadow: 0 0 10px rgba(0, 0, 0, 0.05);"">
                        {Body(htmlContent)}
                        {Footer()}
                    </div>
                </body>
                </html>";
        }

        /// <summary>
        /// Cria o corpo do e-mail, onde o conteúdo principal será exibido.
        /// </summary>
        /// <param name="htmlContent">Conteúdo HTML principal do e-mail.</param>
        /// <returns>Retorna o corpo do e-mail com o conteúdo formatado.</returns>
        private string Body(string htmlContent)
        {
            return $@"
                <div style=""padding: 30px;"">
                    <div style=""padding: 20px; border-radius: 10px;"">
                        <div style=""margin-top: 15px; color: #333333; font-size: 16px;"">
                            {htmlContent}
                        </div>
                    </div>
                </div>";
        }

        /// <summary>
        /// Gera o rodapé do e-mail, informando que a mensagem foi enviada automaticamente e incluindo os direitos autorais.
        /// </summary>
        /// <returns>Retorna o conteúdo do rodapé do e-mail.</returns>
        private string Footer()
        {
            return $@"
                <div style=""color: #888888; font-size: 13px; text-align: center; padding: 15px; background: #dbdbdb; border-top: 1px solid #e0e0e0;"">
                    Esta mensagem foi enviada de forma automática. Por favor, não responda este e-mail.
                </div>";
        }
        #endregion
    }
}
