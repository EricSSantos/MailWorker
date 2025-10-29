using System.Text.Json;

namespace Mail.Service.Commons.Helpers
{
    /// <summary>
    /// Fornece métodos auxiliares para desserialização de payloads de e-mail.
    /// </summary>
    internal static class PayloadHelper
    {
        /// <summary>
        /// Desserializa o payload JSON para o tipo especificado.
        /// </summary>
        /// <typeparam name="TPayload">Tipo esperado do payload.</typeparam>
        /// <param name="payload">Elemento JSON com os dados do payload.</param>
        /// <returns>Instância desserializada do tipo informado.</returns>
        /// <exception cref="InvalidOperationException">Lançada quando o payload é nulo ou inválido.</exception>
        public static TPayload Deserialize<TPayload>(JsonElement? payload)
        {
            if (payload is null || payload.Value.ValueKind == JsonValueKind.Undefined)
                throw new InvalidOperationException("Payload ausente ou indefinido.");

            try
            {
                var result = payload.Value.Deserialize<TPayload>();

                if (result is null)
                    throw new InvalidOperationException("Falha ao desserializar o payload.");

                return result;
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException("Formato inválido no payload.", ex);
            }
        }
    }
}
