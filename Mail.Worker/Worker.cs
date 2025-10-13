using Mail.Service.Interface;

namespace Mail.Worker
{
    public class Worker : BackgroundService
    {
        private readonly IRabbitMQService _rabbitMqService;

        public Worker(IRabbitMQService rabbitMqService)
        {
            _rabbitMqService = rabbitMqService;
        }

        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _rabbitMqService.Start(cancellationToken);
            return Task.CompletedTask;
        }
    }
}
