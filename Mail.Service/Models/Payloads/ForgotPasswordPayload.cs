namespace Mail.Service.Models.Payloads
{
    /// <summary>
    /// Payload específico para e-mails de recuperação de senha,
    /// contendo o código de verificação que será enviado ao usuário.
    /// </summary>
    public sealed class ForgotPasswordPayload
    {
        public int Code { get; init; }
    }
}
