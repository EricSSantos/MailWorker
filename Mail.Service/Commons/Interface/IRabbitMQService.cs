namespace Mail.Service.Commons.Interface
{
    public interface IRabbitMQService
    {
        /// <summary>
        /// Inicia o consumo assíncrono de mensagens da fila configurada no RabbitMQ.
        /// Esse método permanece em execução enquanto o <paramref name="cancellationToken"/> não for cancelado.
        /// </summary>
        /// <param name="cancellationToken">
        /// Token usado para solicitar o cancelamento do processo de consumo, permitindo o desligamento
        /// gracioso do serviço e o fechamento seguro das conexões com o RabbitMQ.
        /// </param>
        void Start(CancellationToken cancellationToken);
    }
}
