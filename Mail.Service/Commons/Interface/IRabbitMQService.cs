namespace Mail.Service.Commons.Interface
{
    /// <summary>
    /// Define o contrato para consumo de mensagens do RabbitMQ.
    /// </summary>
    public interface IRabbitMQService
    {
        /// <summary>
        /// Inicia o consumo assíncrono de mensagens da fila configurada.
        /// Permanece em execução até que o cancelamento seja solicitado.
        /// </summary>
        /// <param name="cancellationToken">Token usado para encerrar o consumo com segurança.</param>
        void Start(CancellationToken cancellationToken);
    }
}
