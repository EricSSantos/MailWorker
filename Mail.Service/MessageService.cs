using Mail.Domain.Entities;
using Mail.Service.Interface;
using Mail.Service.Utils;
using Microsoft.Extensions.Logging;

namespace Mail.Worker.Services
{
    public class MessageService : IMessageService
    {
        #region Dependencies
        private readonly IMailService _mailService;
        private readonly ILogger<MessageService> _logger;

        public MessageService(IMailService mailService, ILogger<MessageService> logger)
        {
            _mailService = mailService;
            _logger = logger;
        }
        #endregion

        /// <summary>
        /// Processa a mensagem de e-mail recebida, deserializando o JSON, validando o conteúdo e enviando o e-mail.
        /// </summary>
        public async Task ProcessMessage(string emailMessageJson)
        {
            if (!ContentHelper.TryDeserializeMessage<EmailMessage>(emailMessageJson, out var emailMessage))
            {
                _logger.LogWarning("Mensagem inválida. Falha ao desserializar JSON para EmailMessage.");
                return;
            }

            try
            {
                await _mailService.SendEmail(emailMessage);
                _logger.LogInformation($"E-mail enviado com sucesso: {emailMessage.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Falha ao processar o e-mail: {emailMessage.Id}");
            }
        }
    }
}
