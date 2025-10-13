using Mail.Domain.Entities;
using Mail.Domain.Enums;
using Mail.Service.Helpers;
using Mail.Service.Interface;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace Mail.Service.Strategies
{
    public sealed class PasswordReset : IEmailStrategy
    {
        private readonly IConfiguration _configuration;

        public PasswordReset(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Domain.Enums.Type Type
        {
            get { return Domain.Enums.Type.PasswordReset; }
        }

        public (string Subject, string HtmlContent) Build(Message message)
        {
            string token = ExtractToken(message.Content);
            const string SUBJECT = "Redefinição de Senha";

            var resetUrl = _configuration["Application:ResetPasswordUrl"];

            var html = $@"
                <h2>Olá, <span style=""color:#27ae60;"">{message.FullName}</span></h2>
                <p>Clique abaixo para redefinir sua senha:</p>
                <a href=""{resetUrl}{token}""
                   style=""background-color:#27ae60;padding:10px 20px;color:white;
                          text-decoration:none;border-radius:5px;font-weight:bold;"">
                    Redefinir Senha
                </a>";

            return (SUBJECT, Template.Wrap(html));
        }

        private static string ExtractToken(string content)
        {
            try
            {
                using var doc = JsonDocument.Parse(content);
                if (doc.RootElement.TryGetProperty("token", out var tokenProp))
                    return tokenProp.GetString() ?? string.Empty;

                throw new ArgumentException("Token não encontrado no conteúdo JSON.");
            }
            catch (JsonException)
            {
                throw new ArgumentException("Formato inválido de conteúdo JSON.");
            }
        }
    }
}
