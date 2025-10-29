using Mail.Service.Commons.Helpers;
using Mail.Service.Commons.Interface;
using Mail.Service.Enums;
using Mail.Service.Models;
using Mail.Service.Models.Payloads;

namespace Mail.Service.Strategies
{
    /// <summary>
    /// Constrói o e-mail de confirmação de conta.
    /// </summary>
    public sealed class ConfirmEmail : IEmailStrategy
    {
        private const string SUBJECT = "Confirmação de e-mail";

        /// <summary>
        /// Obtém o tipo de e-mail suportado pela estratégia.
        /// </summary>
        public EmailType Type
        {
            get { return EmailType.ConfirmEmail; }
        }

        /// <summary>
        /// Gera o conteúdo HTML do e-mail de confirmação.
        /// </summary>
        /// <param name="message">Dados da mensagem de e-mail.</param>
        /// <returns>Assunto e corpo formatado do e-mail.</returns>
        public (string Subject, string HtmlContent) Build(Email message)
        {
            var payload = PayloadHelper.Deserialize<ConfirmEmailPayload>(message.Payload);

            var body = $@"
                <h2>Olá, {message.FullName}!</h2>
                <p>Estamos quase lá! Para ativar sua conta, insira o código abaixo na tela de confirmação:</p>

                <div style=""text-align: center; margin: 20px 0;"">
                    <strong style=""font-size: 24px; letter-spacing: 4px;"">{payload.Code}</strong>
                </div>

                <p>Por segurança, este código expira em poucos minutos.</p>
                <p>Se você não solicitou este cadastro, pode ignorar este e-mail com segurança.</p>
            ";

            var html = TemplateHelper.Wrap(body);

            return (SUBJECT, html);
        }
    }
}
