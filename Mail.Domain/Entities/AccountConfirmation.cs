namespace Mail.Domain.Entities
{
    /// <summary>
    /// Representa a entidade de confirmação de conta, contendo um código único para a validação.
    /// </summary>
    public class AccountConfirmation
    {
        /// <summary>
        /// Código único de confirmação gerado para a validação da conta (Pode receber números e/ou letras).
        /// </summary>
        public string Code { get; set; } = string.Empty;
    }
}
