using System.Text.Json.Serialization;

namespace Nexinho.Models
{
    public class OpenTriviaToken
    {
        [JsonPropertyName("response_code")]
        public int ResponseCode { get; set; }

        [JsonPropertyName("response_message")]
        public string ResponseMessage { get; set; }

        [JsonPropertyName("token")]
        public string Token { get; set; }
    }
}
