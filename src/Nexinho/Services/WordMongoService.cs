using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Nexinho.Commands;
using Nexinho.Models;

namespace Nexinho.Services
{
    public class WordMongoService : IWordMongoService
    {
        private readonly IMongoCollection<Word> wordsDatabase;

        private readonly ILogger logger;

        public WordMongoService(IMongoDatabase mongoDatabase, ILogger<WordMongoService> logger)
        {
            this.wordsDatabase = mongoDatabase.GetCollection<Word>("Words");
            this.logger = logger;
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

        public async Task Reset()
        {
            var filter = Builders<Word>.Filter.Eq(w => w.Solved, true);

            var all = await this.wordsDatabase.Find(filter).ToListAsync();

            foreach (var word in all)
            {
                word.Mask = word.Value.Mask();
                word.Solved = false;

                var filter2 = Builders<Word>.Filter.Eq(w => w.Id, word.Id);
                await this.wordsDatabase.ReplaceOneAsync(filter2, word, new ReplaceOptions { IsUpsert = true });
            }
        }
    }
}
