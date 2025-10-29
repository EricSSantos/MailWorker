using Mail.Service.Commons.Settings;
using Microsoft.Extensions.Options;

namespace Mail.Service.Commons.Helpers
{
    /// <summary>
    /// Fornece métodos auxiliares para geração de templates HTML de e-mail.
    /// </summary>
    public sealed class TemplateHelper
    {
        private readonly ApplicationSettings _app;

        public TemplateHelper(IOptions<ApplicationSettings> options)
        {
            _app = options.Value;
        }

        /// <summary>
        /// Envolve o conteúdo do e-mail com a estrutura HTML padrão e responsiva.
        /// </summary>
        /// <param name="body">Conteúdo principal do e-mail.</param>
        /// <returns>HTML completo com corpo e rodapé formatados.</returns>
        public string Wrap(string body)
        {
            return $@"
                <!DOCTYPE html>
                <html lang=""pt-br"">
                <head>
                    <meta charset=""UTF-8"">
                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                    <title>{_app.Name}</title>
                </head>
                <body style=""margin:0;padding:0;background-color:#f2f4f6;
                                 font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',Roboto,Helvetica,Arial,sans-serif;"">
                    <table width=""100%"" cellpadding=""0"" cellspacing=""0"" role=""presentation"">
                        <tr>
                            <td align=""center"" style=""padding:40px 0;"">
                                <table width=""100%"" cellpadding=""0"" cellspacing=""0"" role=""presentation""
                                       style=""max-width:520px;background:#ffffff;border-radius:12px;
                                              box-shadow:0 3px 10px rgba(0,0,0,0.05);overflow:hidden;"">

                                    <!-- Corpo -->
                                    <tr>
                                        <td style=""padding:30px 40px;"">
                                            {Body(body)}
                                        </td>
                                    </tr>

                                    <!-- Rodapé -->
                                    <tr>
                                        <td style=""padding:20px 30px;background:#f9fafb;text-align:center;border-top:1px solid #eee;"">
                                            {Footer()}
                                        </td>
                                    </tr>

                                </table>
                            </td>
                        </tr>
                    </table>
                </body>
                </html>";
        }

        /// <summary>
        /// Gera o corpo HTML do e-mail.
        /// </summary>
        /// <param name="html">Conteúdo HTML a ser exibido no corpo do e-mail.</param>
        /// <returns>HTML formatado do corpo.</returns>
        private static string Body(string html)
        {
            return $@"
                <div style=""color:#333;font-size:16px;line-height:1.6;text-align:left;"">
                    {html}
                </div>";
        }

        /// <summary>
        /// Gera o rodapé padrão do e-mail.
        /// </summary>
        /// <returns>HTML formatado do rodapé.</returns>
        private string Footer()
        {
            return $@"
                <p style=""margin:0;color:#999;font-size:13px;line-height:1.5;"">
                    Esta mensagem foi gerada automaticamente. Não é necessário respondê-la.
                </p>
                <p style=""margin-top:8px;color:#bbb;font-size:12px;"">
                    © 2025 {_app.Name}. Todos os direitos reservados.
                </p>";
        }
    }
}
