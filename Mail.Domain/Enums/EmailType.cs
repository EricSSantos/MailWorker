using System.ComponentModel;

namespace Mail.Domain.Enums
{
    /// <summary>
    /// Representa os tipos de e-mails que podem ser enviados pelo sistema.
    /// </summary>
    public enum EmailType
    {
        /// <summary>
        /// E-mail enviado para confirmação de cadastro do usuário.
        /// </summary>
        AccountConfirmation,
        /// <summary>
        /// E-mail enviado para redefinição de senha do usuário.
        /// </summary>
        PasswordReset
    }
}
