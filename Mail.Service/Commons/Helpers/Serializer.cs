using System.Text.Json;

namespace Mail.Service.Commons.Helpers
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
        /// Serializa um objeto para JSON.
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
            WriteIndented = false
        };

        #endregion
    }
}
