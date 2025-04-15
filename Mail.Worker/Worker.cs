using Mail.Service.Interface;

namespace Mail.Worker
{
    public class Worker : BackgroundService
    {
        #region Dependencies
        private readonly IRabbitMQService _rabbitMqService;

        /// <summary>
        /// Construtor da classe Worker que recebe a instância do serviço de RabbitMQ.
        /// </summary>
        /// <param name="rabbitMqService">Serviço responsável por consumir mensagens da fila.</param>
        public Worker(IRabbitMQService rabbitMqService)
        {
            _rabbitMqService = rabbitMqService;
        }
        #endregion

        /// <summary>
        /// Método executado quando o serviço em segundo plano é iniciado.
        /// Inicia o consumo de mensagens da fila RabbitMQ.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelamento do serviço.</param>
        /// <returns>Uma tarefa concluída.</returns>
        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _rabbitMqService.Start(cancellationToken);
            return Task.CompletedTask;
        }
    }
}
