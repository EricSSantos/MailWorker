using System.Text.Json;
using System.Text.Json.Serialization;

namespace Mail.Service.Helpers
{
    public static class Serializer
    {
        /// <summary>
        /// Desserializa uma mensagem JSON para o tipo especificado.
        /// </summary>
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

        /// <summary>
        /// Tenta serializar um objeto para JSON.
        /// </summary>
        public static bool TrySerializeMessage<T>(T value, out string? json)
        {
            try
            {
                json = JsonSerializer.Serialize(value, MessageOptions);
                return true;
            }
            catch
            {
                json = default;
                return false;
            }
        }

        #region Private Methods

        private static readonly JsonSerializerOptions MessageOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = false,
            Converters = { new JsonStringEnumConverter() }
        };

        #endregion
    }
}
