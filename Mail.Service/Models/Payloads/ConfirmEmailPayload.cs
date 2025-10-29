namespace Mail.Service.Models.Payloads
{
    /// <summary>
    /// Representa o payload do e-mail de confirmação de conta.
    /// </summary>
    public sealed class ConfirmEmailPayload
    {
        /// <summary>
        /// Código de verificação enviado ao usuário.
        /// </summary>
        public int Code { get; init; }
    }
}
