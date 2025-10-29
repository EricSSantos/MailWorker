namespace Mail.Service.Commons.Settings
{
    /// <summary>
    /// Define as configurações de envio de e-mails via SMTP.
    /// </summary>
    public sealed class SmtpSettings
    {
        /// <summary>
        /// Endereço do servidor SMTP.
        /// </summary>
        public string Host { get; set; } = string.Empty;

        /// <summary>
        /// Porta de conexão do servidor SMTP.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Usuário para autenticação no servidor SMTP.
        /// </summary>
        public string User { get; set; } = string.Empty;

        /// <summary>
        /// Senha para autenticação no servidor SMTP.
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Endereço de e-mail do remetente.
        /// </summary>
        public string FromEmail { get; set; } = string.Empty;

        /// <summary>
        /// Nome exibido como remetente.
        /// </summary>
        public string FromName { get; set; } = string.Empty;

        /// <summary>
        /// Indica se a conexão SSL está habilitada.
        /// </summary>
        public bool EnableSsl { get; set; } = true;
    }
}
