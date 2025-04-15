using Mail.Domain.Entities;
using Mail.Service.Interface;
using Mail.Service.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Mail.Worker.Services
{
    public class RabbitMQService : IRabbitMQService, IDisposable
    {
        #region Dependencies
        private readonly IConfiguration _configuration;
        private readonly IMessageService _messageService;
        private readonly ILogger<RabbitMQService> _logger;

        private readonly IConnection _connection;
        private readonly IModel _channel;

        private const int MAX_RETRY = 3;
        private const string RETRY_HEADER = "x-retry-count";
        private const string ERROR_HEADER = "x-error";

        public RabbitMQService(IConfiguration config, IMessageService messageService, ILogger<RabbitMQService> logger)
        {
            _configuration = config;
            _messageService = messageService;
            _logger = logger;

            var rabbit = _configuration.GetSection("RabbitMQ");

            var factory = new ConnectionFactory
            {
                HostName = rabbit["Host"],
                Port = int.Parse(rabbit["Port"]!),
                UserName = rabbit["User"],
                Password = rabbit["Password"],
                DispatchConsumersAsync = true
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: rabbit["MailQueue"], durable: true, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueDeclare(queue: rabbit["DeadLetterQueue"], durable: true, exclusive: false, autoDelete: false, arguments: null);
        }
        #endregion

        /// <summary>
        /// Inicia o consumo de mensagens da fila.
        /// </summary>
        public void Start(CancellationToken cancellationToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.Received += async (_, ea) =>
            {
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                var retryCount = RetryCount(ea);

                if (!ContentHelper.TryDeserializeMessage<EmailMessage>(json, out var emailMessage))
                {
                    _logger.LogWarning($"Mensagem inválida. Falha ao desserializar JSON para EmailMessage.");
                    SendToDeadLetter(json, "Falha ao desserializar JSON para EmailMessage.");
                    _channel.BasicAck(ea.DeliveryTag, false);
                    return;
                }

                try
                {
                    _logger.LogInformation($"Processando mensagem: {emailMessage.Id}");

                    await _messageService.ProcessMessage(json);

                    _channel.BasicAck(ea.DeliveryTag, false);
                    _logger.LogInformation($"Mensagem processada com sucesso: {emailMessage.Id}");
                }
                catch (Exception ex)
                {
                    retryCount++;

                    var id = emailMessage?.Id.ToString();

                    if (retryCount >= MAX_RETRY)
                    {
                        _logger.LogError(ex, $"Falha após {retryCount} tentativas. Enviando para dead-letter: {id}");
                        SendToDeadLetter(json, $"Falha após {MAX_RETRY} tentativas: {ex.Message}");
                    }
                    else
                    {
                        _logger.LogWarning($"Falha ao processar mensagem {id}. Reenviando. Tentativa: {retryCount}");
                        RetryMessage(json, retryCount);
                    }

                    _channel.BasicAck(ea.DeliveryTag, false);
                }
            };

            _channel.BasicConsume(queue: _configuration["RabbitMQ:MailQueue"]!, autoAck: false, consumer: consumer);
        }

        /// <summary>
        /// Fecha conexões e canais do RabbitMQ.
        /// </summary>
        public void Dispose()
        {
            _channel?.Close();
            _channel?.Dispose();
            _connection?.Close();
            _connection?.Dispose();
        }

        #region Private Methods
        /// <summary>
        /// Obtém o número de tentativas de reenvio de uma mensagem a partir de seus headers.
        /// </summary>
        private int RetryCount(BasicDeliverEventArgs ea)
        {
            if (ea.BasicProperties.Headers != null &&
                ea.BasicProperties.Headers.TryGetValue(RETRY_HEADER, out var retryHeader) &&
                retryHeader is byte[] retryBytes &&
                int.TryParse(Encoding.UTF8.GetString(retryBytes), out int retryCount))
            {
                return retryCount;
            }

            return 0;
        }

        /// <summary>
        /// Reenvia a mensagem para a fila de origem, incrementando o contador de tentativas.
        /// </summary>
        private void RetryMessage(string json, int retryCount)
        {
            var props = _channel.CreateBasicProperties();
            props.Persistent = true;
            props.Headers = new Dictionary<string, object>
            {
                { RETRY_HEADER, Encoding.UTF8.GetBytes(retryCount.ToString()) }
            };

            _channel.BasicPublish(
                exchange: "",
                routingKey: _configuration["RabbitMQ:MailQueue"],
                basicProperties: props,
                body: Encoding.UTF8.GetBytes(json)
            );
        }

        /// <summary>
        /// Envia a mensagem para a fila de dead letter quando o número máximo de tentativas é atingido.
        /// </summary>
        private void SendToDeadLetter(string json, string errorMessage)
        {
            var props = _channel.CreateBasicProperties();
            props.Persistent = true;
            props.Headers = new Dictionary<string, object>
            {
                { ERROR_HEADER, Encoding.UTF8.GetBytes(errorMessage) }
            };

            _channel.BasicPublish(
                exchange: "",
                routingKey: _configuration["RabbitMQ:DeadLetterQueue"],
                basicProperties: props,
                body: Encoding.UTF8.GetBytes(json)
            );
        }
        #endregion
    }
}
