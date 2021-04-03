using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using Nexinho.Models;

namespace Nexinho.Services
{
    public class MongoService : IWordService
    {
        private readonly IMongoCollection<Word> wordsDatabase;
        private readonly IMongoCollection<Ranking> rankingDatabase;

        public MongoService(IMongoDatabase mongoDatabase)
        {
            this.wordsDatabase = mongoDatabase.GetCollection<Word>("Words");
            this.rankingDatabase = mongoDatabase.GetCollection<Ranking>("Ranking");
        }

        public async Task<Word> GetCurrent()
        {
            var filter = Builders<Word>.Filter.Eq(w => w.Current, true);

            return await this.wordsDatabase.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<Word> GetNext()
        {
            // todo randomize the word pick
            var filter = Builders<Word>.Filter.Eq(w => w.Solved, false);

            return await this.wordsDatabase.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<bool> InsertWord(Word word)
        {
            var filter = Builders<Word>.Filter.Eq(w => w.Value, word.Value);

            var result = await this.wordsDatabase.Find(filter).FirstOrDefaultAsync();

            if (result != null)
            {
                return false;
            }

            await this.wordsDatabase.InsertOneAsync(word);

            return true;
        }

        public async Task UpdateWord(Word word)
        {
            var filter = Builders<Word>.Filter.Eq(w => w.Id, word.Id);

            var result = await this.wordsDatabase.ReplaceOneAsync(filter, word);
        }

        public async Task<Ranking> GetCurrentRanking()
        {
            var filter = Builders<Ranking>.Filter.Eq(w => w.Id, $"{DateTime.Now.Year}-{DateTime.Now.Month}");

            var current = await this.rankingDatabase.Find(filter).FirstOrDefaultAsync();

            if (current == null)
            {
                var ranking = new Ranking { Id = $"{DateTime.Now.Year}-{DateTime.Now.Month}", Ranks = new List<Rank>() };

                return ranking;
            }

            return current;
        }

        public async Task UpdateCurrentRanking(Ranking rank)
        {
            var filter = Builders<Ranking>.Filter.Eq(w => w.Id, rank.Id);
            var update = Builders<Ranking>.Update.Set(r => r.Ranks, rank.Ranks);

            await this.rankingDatabase.UpdateOneAsync(filter, update);
        }
    }
}
