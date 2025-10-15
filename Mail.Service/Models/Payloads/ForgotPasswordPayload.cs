namespace Mail.Service.Models.Payloads
{
    /// <summary>
    /// Payload específico para e-mails de recuperação de senha,
    /// contendo o código de verificação "OTP" que será enviado ao usuário.
    /// </summary>
    public sealed class ForgotPasswordPayload
    {
        public string Code { get; init; } = string.Empty;
    }
}
