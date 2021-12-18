using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Nexinho.Models;

namespace Nexinho.Services;

public class RankMongoService
{
    private readonly IMongoCollection<Ranking> _rankingDatabase;
    private readonly ILogger<RankMongoService> _logger;

    public RankMongoService(IMongoDatabase mongoDatabase, ILogger<RankMongoService> logger)
    {
        _rankingDatabase = mongoDatabase.GetCollection<Ranking>("Ranking");
        _logger = logger;
    }

    public async Task Update(Ranking rank)
    {
        var filter = Builders<Ranking>.Filter.Eq(w => w.Id, rank.Id);
        var update = Builders<Ranking>.Update.Set(r => r.Ranks, rank.Ranks);

        await _rankingDatabase.UpdateOneAsync(filter, update);

        _logger.LogInformation("rank updated", rank);
    }

    public async Task<Ranking> GetOrSet(RankCategory category)
    {
        var filter = Builders<Ranking>.Filter.Eq(w => w.Id, $"{category.ToString()}-{DateTime.Now.Year}-{DateTime.Now.Month}");

        try
        {
            var current = await _rankingDatabase.Find(filter).FirstOrDefaultAsync();

            if (current == null)
            {
                var ranking = new Ranking
                {
                    Id = $"{DateTime.Now.Year}-{DateTime.Now.Month}",
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
}