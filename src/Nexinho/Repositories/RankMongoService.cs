using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Nexinho.Models;

namespace Nexinho.Repositories;

public class RankMongoService
{
    private readonly IMongoCollection<Ranking> _rankingDatabase;
    private readonly ILogger<RankMongoService> _logger;

    public RankMongoService(IMongoDatabase mongoDatabase, ILogger<RankMongoService> logger)
    {
        _rankingDatabase = mongoDatabase.GetCollection<Ranking>("Ranking");
        _logger = logger;
    }

    public async Task Update(Ranking ranking)
    {
        var filter = Builders<Ranking>.Filter.Eq(w => w.Id, ranking.Id);
        var update = Builders<Ranking>.Update.Set(r => r.Ranks, ranking.Ranks);

        await _rankingDatabase.UpdateOneAsync(filter, update);

        _logger.LogInformation("ranking updated", ranking);
    }

    public async Task SetRank(RankCategory category, string username, int points)
    {
        var ranking = await GetOrSet(category);

        if (ranking.Ranks.Any(r => r.Username == username))
        {
            ranking.Ranks.First(r => r.Username == username).Points += points;

            _logger.LogInformation("ranked user found", ranking);
        }
        else
        {
            ranking.Ranks.Add(new Rank
            {
                Username = username,
                Points = points
            });

            _logger.LogInformation("ranked user created", ranking);
        }

        await Update(ranking);
    }

    public async Task<Ranking> GetOrSet(RankCategory category)
    {
        var filter = Builders<Ranking>.Filter.Eq(w => w.Id, GenerateId(category));

        try
        {
            var current = await _rankingDatabase.Find(filter).FirstOrDefaultAsync();

            if (current == null)
            {
                var ranking = new Ranking
                {
                    Id = GenerateId(category),
                    Category = category,
                    Ranks = new List<Rank>()
                };

                await _rankingDatabase.InsertOneAsync(ranking);

                _logger.LogInformation("rank not found. creating a new rank", ranking);

                return ranking;
            }

            _logger.LogInformation("rank found", current);

            return current;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex.Message);

            return null;
        }
    }

    private string GenerateId(RankCategory category)
    {
        return $"{category.ToString()}-{DateTime.Now.Year}-{DateTime.Now.Month}";
    }
}