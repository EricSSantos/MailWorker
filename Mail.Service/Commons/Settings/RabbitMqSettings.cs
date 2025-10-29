namespace Mail.Service.Commons.Settings
{
    /// <summary>
    /// Define as configurações de conexão e filas do RabbitMQ.
    /// </summary>
    public sealed class RabbitMqSettings
    {
        /// <summary>
        /// Endereço do host do RabbitMQ.
        /// </summary>
        public string Host { get; set; } = string.Empty;

        /// <summary>
        /// Porta de conexão do RabbitMQ.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Usuário para autenticação no RabbitMQ.
        /// </summary>
        public string User { get; set; } = string.Empty;

        /// <summary>
        /// Senha para autenticação no RabbitMQ.
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Nome da fila principal de envio de e-mails.
        /// </summary>
        public string EmailQueue { get; set; } = string.Empty;

        /// <summary>
        /// Nome da fila de mensagens não processadas (Dead Letter).
        /// </summary>
        public string DeadLetterQueue { get; set; } = string.Empty;
    }
}
