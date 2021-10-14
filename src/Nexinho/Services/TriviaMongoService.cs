using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Nexinho.Models;

namespace Nexinho.Services
{
    public class TriviaMongoService : ITriviaMongoService
    {
        private readonly IMongoCollection<Ranking> questionsRank;
        private readonly ILogger logger;

        public TriviaMongoService(IMongoDatabase mongoDatabase, ILogger<TriviaMongoService> logger)
        {
            if (mongoDatabase is null)
            {
                throw new ArgumentNullException(nameof(mongoDatabase));
            }

            this.questionsRank = mongoDatabase.GetCollection<Ranking>("questions-rank");

            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Ranking> Get()
        {
            var filter = Builders<Ranking>.Filter.Eq(w => w.Id, $"{DateTime.Now.Year}-{DateTime.Now.Month}");

            try
            {
                var current = await this.questionsRank.Find(filter).FirstOrDefaultAsync();

                if (current == null)
                {
                    var ranking = new Ranking { Id = $"{DateTime.Now.Year}-{DateTime.Now.Month}", Ranks = new List<Rank>() };

                    await this.questionsRank.InsertOneAsync(ranking);

                    return ranking;
                }

                return current;
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex.Message);

                return null;
            }
        }

        public async Task Update(Ranking rank)
        {
            var filter = Builders<Ranking>.Filter.Eq(w => w.Id, rank.Id);
            var update = Builders<Ranking>.Update.Set(r => r.Ranks, rank.Ranks);

            await this.questionsRank.UpdateOneAsync(filter, update);
        }
    }
}

