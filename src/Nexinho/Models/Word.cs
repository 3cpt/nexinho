using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Nexinho.Models
{
    public class Word
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public bool Current { get; set; }

        public bool Solved { get; set; }

        public string Value { get; set; }

        public string Mask { get; set; }
    }
}