using Mail.Service.Commons.Helpers;
using Mail.Service.Commons.Interface;
using Mail.Service.Commons.Settings;
using Mail.Service.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Mail.Service
{
    public sealed class RabbitMQService : IRabbitMQService, IDisposable
    {
        private readonly RabbitMqSettings _settings;
        private readonly IEmailService _mailService;
        private readonly ILogger<RabbitMQService> _logger;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        private const int MAX_RETRY = 3;
        private const string RETRY_HEADER = "x-retry-count";
        private const string ERROR_HEADER = "x-error";

        public RabbitMQService(
            IOptions<RabbitMqSettings> options,
            IEmailService mailService,
            ILogger<RabbitMQService> logger)
        {
            _settings = options.Value;
            _mailService = mailService;
            _logger = logger;

            var factory = new ConnectionFactory
            {
                HostName = _settings.Host,
                Port = _settings.Port,
                UserName = _settings.User,
                Password = _settings.Password,
                DispatchConsumersAsync = true
            };

            for (int attempt = 1; attempt <= MAX_RETRY; attempt++)
            {
                try
                {
                    _logger.LogInformation(
                        "Tentando conectar ao RabbitMQ (tentativa {Attempt}/{Max}) em {Host}:{Port}...",
                        attempt, MAX_RETRY, _settings.Host, _settings.Port);

                    _connection = factory.CreateConnection();
                    _channel = _connection.CreateModel();

                    _logger.LogInformation("Conexão com RabbitMQ estabelecida com sucesso.");
                    DeclareQueues();
                    return;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Falha ao conectar ao RabbitMQ (tentativa {Attempt}/{Max}) em {Host}:{Port}",
                        attempt, MAX_RETRY, _settings.Host, _settings.Port);

                    if (attempt == MAX_RETRY)
                        throw;

                    var delay = TimeSpan.FromSeconds(Math.Pow(2, attempt));
                    _logger.LogWarning("Aguardando {Delay}s antes da próxima tentativa...", delay.TotalSeconds);
                    Thread.Sleep(delay);
                }
            }
        }

        private void DeclareQueues()
        {
            _channel.QueueDeclare(
                queue: _settings.EmailQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            _channel.QueueDeclare(
                queue: _settings.DeadLetterQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            _logger.LogInformation(
                "Filas declaradas com sucesso: {EmailQueue}, {DeadLetterQueue}",
                _settings.EmailQueue, 
                _settings.DeadLetterQueue);
        }

        public void Start(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Iniciando consumo de mensagens da fila {Queue}", _settings.EmailQueue);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += async (_, ea) =>
            {
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                var retryCount = GetRetryCount(ea);

                if (!Serializer.TryDeserializeMessage<Email>(json, out var emailMessage))
                {
                    _logger.LogWarning("Mensagem inválida recebida. Falha ao desserializar JSON para EmailMessage.");
                    
                    SendToDeadLetter(json, "Falha ao desserializar JSON.");
                    _channel.BasicAck(ea.DeliveryTag, false);
                    
                    return;
                }

                try
                {
                    _logger.LogInformation("Processando e-mail para {To}", emailMessage.To);
                    
                    await _mailService.SendEmail(emailMessage);
                    _channel.BasicAck(ea.DeliveryTag, false);
                    
                    _logger.LogInformation("E-mail enviado com sucesso para {To}", emailMessage.To);
                }
                catch (Exception ex)
                {
                    retryCount++;

                    if (retryCount >= MAX_RETRY)
                    {
                        _logger.LogError(ex,
                            "Falha após {RetryCount} tentativas. Enviando mensagem para DeadLetter ({To})",
                            retryCount, emailMessage.To);

                        SendToDeadLetter(json, $"Falha após {MAX_RETRY} tentativas: {ex.Message}");
                    }
                    else
                    {
                        _logger.LogWarning(
                            "Falha ao processar e-mail para {To}. Reenviando tentativa {RetryCount}/{Max}.",
                            emailMessage.To, retryCount, MAX_RETRY);

                        RetryMessage(json, retryCount);
                    }

                    _channel.BasicAck(ea.DeliveryTag, false);
                }
            };

            _channel.BasicConsume(
                queue: _settings.EmailQueue,
                autoAck: false,
                consumer: consumer);

            _logger.LogInformation("RabbitMQ Consumer iniciado com sucesso e aguardando mensagens.");
        }

        public void Dispose()
        {
            _logger.LogInformation("Encerrando conexão com RabbitMQ...");
            
            _channel?.Close();
            _channel?.Dispose();
            _connection?.Close();
            _connection?.Dispose();
            
            _logger.LogInformation("Conexão RabbitMQ encerrada.");
        }

        private int GetRetryCount(BasicDeliverEventArgs ea)
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
                routingKey: _settings.EmailQueue,
                basicProperties: props,
                body: Encoding.UTF8.GetBytes(json));
        }

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
                routingKey: _settings.DeadLetterQueue,
                basicProperties: props,
                body: Encoding.UTF8.GetBytes(json));
        }
    }
}
