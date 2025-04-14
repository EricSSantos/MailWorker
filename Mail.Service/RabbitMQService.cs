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
    public class RabbitMQService : IRabbitMQService
    {
        #region Dependencies
        private readonly string _mailQueue;
        private readonly string _deadLetterQueue;
        private readonly string _host;
        private readonly int _port;
        private readonly string _user;
        private readonly string _password;

        private readonly IModel _channel;
        private readonly IConnection _connection;
        private readonly IMessageService _messageService;
        private readonly ILogger<RabbitMQService> _logger;

        private const int MAX_RETRY = 3;

        /// <summary>
        /// Construtor do serviço, configura as dependências e inicializa a conexão RabbitMQ.
        /// </summary>
        public RabbitMQService(IConfiguration config, IMessageService messageService, ILogger<RabbitMQService> logger)
        {
            _messageService = messageService;
            _logger = logger;

            var rabbit = config.GetSection("RabbitMQ");

            _mailQueue = rabbit["MailQueue"]!;
            _deadLetterQueue = rabbit["DeadLetterQueue"]!;
            _host = rabbit["Host"]!;
            _port = int.Parse(rabbit["Port"]!);
            _user = rabbit["User"]!;
            _password = rabbit["Password"]!;

            var factory = new ConnectionFactory
            {
                HostName = _host,
                Port = _port,
                UserName = _user,
                Password = _password
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: _mailQueue, durable: true, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueDeclare(queue: _deadLetterQueue, durable: true, exclusive: false, autoDelete: false, arguments: null);
        }
        #endregion

        /// <summary>
        /// Inicia o serviço de consumo de mensagens do RabbitMQ e processa as mensagens recebidas.
        /// </summary>
        public void Start(CancellationToken cancellationToken)
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (_, ea) =>
            {
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                var emailMessage = ContentHelper.DeserializeMessage<EmailMessage>(json);
                var retryCount = RetryCount(ea);

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

                    if (retryCount >= MAX_RETRY)
                    {
                        _logger.LogError(ex, $"Mensagem falhou após {MAX_RETRY} tentativas. Enviando para a fila de dead letter: {emailMessage.Id}");
                        SendToDeadLetter(json, $"Mensagem falhou após {MAX_RETRY} tentativas. Erros: {ex}");
                    }
                    else
                    {
                        _logger.LogWarning($"Falha no processamento da mensagem. Tentando novamente: {emailMessage.Id}, Tentativa: {retryCount}");
                        RetryMessage(json, retryCount);
                    }

                    _channel.BasicAck(ea.DeliveryTag, false);
                }
            };

            _channel.BasicConsume(_mailQueue, autoAck: false, consumer: consumer);
        }

        #region Private Methods
        /// <summary>
        /// Obtém o número de tentativas de reenvio de uma mensagem a partir de seus headers.
        /// </summary>
        private int RetryCount(BasicDeliverEventArgs ea)
        {
            if (ea.BasicProperties.Headers != null &&
                ea.BasicProperties.Headers.TryGetValue("x-retry-count", out var retryHeader) &&
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
                { "x-retry-count", Encoding.UTF8.GetBytes(retryCount.ToString()) }
            };

            _channel.BasicPublish(exchange: "", routingKey: _mailQueue, basicProperties: props, body: Encoding.UTF8.GetBytes(json));
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
                { "x-error", Encoding.UTF8.GetBytes(errorMessage) }
            };

            _channel.BasicPublish(exchange: "", routingKey: _deadLetterQueue, basicProperties: props, body: Encoding.UTF8.GetBytes(json));
        }
        #endregion
    }
}
