namespace Mail.Service.Commons.Helpers
{
    public static class Template
    {
        public static string Wrap(string body)
        {
            return $@"
                <!DOCTYPE html>
                <html lang=""pt-br"">
                <head>
                    <meta charset=""UTF-8"">
                    <meta name=""viewport"" content=""width=device-width,initial-scale=1.0"">
                </head>
                <body style=""margin:0;padding:0;font-family:'Segoe UI'"">
                    <div style=""margin:40px auto;max-width:500px;background-color:#f7f7f7;border-radius:10px;
                                overflow:hidden;box-shadow:0 0 10px rgba(0,0,0,0.05);"">
                        {Body(body)}
                        {Footer()}
                    </div>
                </body>
                </html>";
        }

        private static string Body(string html)
        {
            return $@"
                <div style=""padding:30px;"">
                    <div style=""padding:20px;"">
                        <div style=""color:#333;font-size:16px;"">
                            {html}
                        </div>
                    </div>
                </div>";
        }

        private static string Footer()
        {
            return @"
                <div style=""color:#888;font-size:13px;text-align:center;padding:15px;background:#dbdbdb;
                            border-top:1px solid #e0e0e0;"">
                    Esta mensagem foi enviada automaticamente. Por favor, não responda.
                </div>";
        }
    }
}
