using System.Text.Json;
using System.Text.Json.Serialization;

namespace Mail.Service.Utils
{
    public static class ContentHelper
    {
        /// <summary>
        /// Desserializa o conteúdo JSON de um <see cref="JsonElement"/> para o tipo especificado.
        /// </summary>
        /// <typeparam name="T">Tipo de destino para a desserialização.</typeparam>
        /// <param name="content">Conteúdo JSON a ser desserializado.</param>
        /// <param name="result">Objeto desserializado.</param>
        /// <returns>True se a desserialização foi bem-sucedida, caso contrário false.</returns>
        public static bool TryDeserializeContent<T>(JsonElement content, out T? result)
        {
            try
            {
                result = content.Deserialize<T>(ContentOptions);
                return result != null;
            }
            catch
            {
                result = default;
                return false;
            }
        }

        /// <summary>
        /// Desserializa uma mensagem JSON (string) para o tipo especificado.
        /// </summary>
        /// <typeparam name="T">Tipo de destino para a desserialização.</typeparam>
        /// <param name="json">Mensagem JSON a ser desserializada.</param>
        /// <param name="result">Objeto desserializado.</param>
        /// <returns>True se a desserialização foi bem-sucedida, caso contrário false.</returns>
        public static bool TryDeserializeMessage<T>(string json, out T? result)
        {
            try
            {
                result = JsonSerializer.Deserialize<T>(json, MessageOptions);
                return result != null;
            }
            catch
            {
                result = default;
                return false;
            }
        }

        #region Private Methods
        private static readonly JsonSerializerOptions ContentOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private static readonly JsonSerializerOptions MessageOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };
        #endregion
    }
}
