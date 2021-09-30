using System.Text.Json.Serialization;

namespace Nexinho.Models
{
    public class EvilInsult
    {
        [JsonPropertyName("number")]
        public string Number { get; set; }

        [JsonPropertyName("language")]
        public string Language { get; set; }

        [JsonPropertyName("insult")]
        public string Insult { get; set; }

        [JsonPropertyName("created")]
        public string Created { get; set; }

        [JsonPropertyName("shown")]
        public string Shown { get; set; }

        [JsonPropertyName("createdby")]
        public string Createdby { get; set; }

        [JsonPropertyName("active")]
        public string Active { get; set; }

        [JsonPropertyName("comment")]
        public string Comment { get; set; }
    }
}
