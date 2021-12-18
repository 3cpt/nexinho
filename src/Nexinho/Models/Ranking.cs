using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace Nexinho.Models;

public class Ranking
{
    [BsonId]
    public string Id { get; set; }

    public RankCategory Category { get; set; }

    public List<Rank> Ranks { get; set; }
}
