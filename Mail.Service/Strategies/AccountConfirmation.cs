using Mail.Domain.Entities;
using Mail.Domain.Enums;
using Mail.Service.Helpers;
using Mail.Service.Interface;
using System.Text.Json;

namespace Mail.Service.Strategies
{
    public sealed class AccountConfirmation : IEmailStrategy
    {
        public Domain.Enums.Type Type
        {
            get { return Domain.Enums.Type.AccountConfirmation; }
        }

        public (string Subject, string HtmlContent) Build(Message message)
        {
            string code = ExtractCode(message.Content);
            const string SUBJECT = "Confirmação de Cadastro";

            var html = $@"
                <h2>Seja bem-vindo, <span style=""color:#27ae60;"">{message.FullName}</span></h2>
                <p>Seu cadastro foi realizado com sucesso.</p>
                <p>Utilize o código abaixo para confirmar:</p>
                <div style='font-size:24px;font-weight:bold;letter-spacing:4px;background-color:#27ae60;
                            padding:15px;border-radius:8px;margin:20px auto;text-align:center;color:white;'>
                    {code}
                </div>";

            return (SUBJECT, Template.Wrap(html));
        }

        private static string ExtractCode(string content)
        {
            try
            {
                using var doc = JsonDocument.Parse(content);
                if (doc.RootElement.TryGetProperty("code", out var codeProp))
                    return codeProp.GetString() ?? string.Empty;

                throw new ArgumentException("Código de confirmação não encontrado no conteúdo JSON.");
            }
            catch (JsonException)
            {
                throw new ArgumentException("Formato inválido de conteúdo JSON.");
            }
        }
    }
}
