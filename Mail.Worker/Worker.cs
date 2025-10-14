using Mail.Service.Commons.Interface;

namespace Mail.Worker
{
    public sealed class Worker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public Worker(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var rabbitMqService = scope.ServiceProvider.GetRequiredService<IRabbitMQService>();

            rabbitMqService.Start(stoppingToken);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}
