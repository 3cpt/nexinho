using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Nexinho.Models
{
    public class OpenTriviaResult
    {
        [JsonPropertyName("response_code")]
        public int ResponseCode { get; set; }

        [JsonPropertyName("results")]
        public List<OpenTrivia> Results { get; set; }
    }
}

