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
        /// <returns>Objeto desserializado do tipo <typeparamref name="T"/>.</returns>
        /// <exception cref="ArgumentException">Lançado se a desserialização falhar.</exception>
        public static T DeserializeContent<T>(JsonElement content)
        {
            return content.Deserialize<T>(ContentOptions)
                   ?? throw new ArgumentException($"Falha ao desserializar conteúdo para {typeof(T).Name}");
        }

        /// <summary>
        /// Desserializa uma mensagem JSON (string) para o tipo especificado.
        /// </summary>
        /// <typeparam name="T">Tipo de destino para a desserialização.</typeparam>
        /// <param name="json">Mensagem JSON a ser desserializada.</param>
        /// <returns>Objeto desserializado do tipo <typeparamref name="T"/>.</returns>
        /// <exception cref="ArgumentException">Lançado se a desserialização falhar.</exception>
        public static T DeserializeMessage<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json, MessageOptions)
                   ?? throw new ArgumentException($"Falha ao desserializar mensagem para {typeof(T).Name}");
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
