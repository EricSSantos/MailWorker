using System.ComponentModel;

namespace Mail.Service.Enums
{
    public enum EmailType
    {
        [Description("E-mail de boas-vindas")]
        Welcome = 0,
        [Description("E-mail de recuperação de senha")]
        ForgotPassword = 1
    }
}
