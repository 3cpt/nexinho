using System.Collections.Generic;
using System.Net;
using System.Text.Json.Serialization;

namespace Nexinho.Models
{
    public class OpenTrivia
    {
        private string question;
        private string correctAnswer;
        private List<string> incorrectAnswers;

        [JsonPropertyName("category")]
        public string Category { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("difficulty")]
        public string Difficulty { get; set; }

        [JsonPropertyName("question")]
        public string Question { get => question; set => question = WebUtility.HtmlDecode(value); }

        [JsonPropertyName("correct_answer")]
        public string CorrectAnswer { get => correctAnswer; set => correctAnswer = WebUtility.HtmlDecode(value); }

        [JsonPropertyName("incorrect_answers")]
        public List<string> IncorrectAnswers { get => incorrectAnswers; set => incorrectAnswers = value.ConvertAll(a => WebUtility.HtmlDecode(a)); }

        public bool Answered { get; set; }
    }
}
