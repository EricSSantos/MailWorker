namespace Mail.Service.Commons.Interface
{
    public interface IRabbitMQService
    {
        /// <summary>
        /// Inicia o consumo de mensagens da fila RabbitMQ.
        /// </summary>
        void Start(CancellationToken cancellationToken);
    }
}
