namespace Mail.Domain.Entities
{
    /// <summary>
    /// Representa uma solicitação de redefinição de senha, contendo um token único para validação.
    /// </summary>
    public class PasswordReset
    {
        /// <summary>
        /// Token único gerado para validar a solicitação de redefinição de senha.
        /// </summary>
        public string Token { get; set; } = string.Empty;
    }
}
