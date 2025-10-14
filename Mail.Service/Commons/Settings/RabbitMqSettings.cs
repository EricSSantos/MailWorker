namespace Mail.Service.Commons.Settings
{
    public sealed class RabbitMqSettings
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }
        public string User { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string EmailQueue { get; set; } = string.Empty;
        public string DeadLetterQueue { get; set; } = string.Empty;
    }
}
