using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace Nexinho.Models
{
    public class Ranking
    {
        [BsonId]
        public string Id { get; set; }

        public RankCategory Category { get; set; }

        public List<Rank> Ranks { get; set; }
    }

    public class Rank
    {
        public string Username { get; set; }

        public int Points { get; set; }
    }

    public enum RankCategory
    {
        Words = 1,
        Trivia = 2
    }
}