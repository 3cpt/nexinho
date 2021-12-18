using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Nexinho.Extensions;
using Nexinho.Models;

namespace Nexinho.Repositories;

public class WordMongoService : IWordMongoService
{
    private readonly IMongoCollection<Word> _wordsDatabase;

    private readonly ILogger _logger;

    public WordMongoService(IMongoDatabase mongoDatabase, ILogger<WordMongoService> logger)
    {
        _wordsDatabase = mongoDatabase.GetCollection<Word>("Words");
        _logger = logger;
    }

    public async Task<Word> GetCurrent()
    {
        var filter = Builders<Word>.Filter.Eq(w => w.Current, true);

        _logger.LogInformation("trying to get a word", filter);

        return await this._wordsDatabase.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<Word> GetNext()
    {
        // todo randomize the word pick
        var filter = Builders<Word>.Filter.Eq(w => w.Solved, false);

        _logger.LogInformation("trying to get the next word", filter);

        return await this._wordsDatabase.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<bool> InsertWord(Word word)
    {
        var filter = Builders<Word>.Filter.Eq(w => w.Value, word.Value);

        var result = await this._wordsDatabase.Find(filter).FirstOrDefaultAsync();

        if (result != null)
        {
            _logger.LogInformation("word already in the database", word);

            return false;
        }

        await this._wordsDatabase.InsertOneAsync(word);

        _logger.LogInformation("the word selected added to the database", word);

        return true;
    }

    public async Task UpdateWord(Word word)
    {
        var filter = Builders<Word>.Filter.Eq(w => w.Id, word.Id);

        _logger.LogInformation("trying to get update a word", word);

        var result = await this._wordsDatabase.ReplaceOneAsync(filter, word);
    }

    public async Task Reset()
    {
        var filter = Builders<Word>.Filter.Eq(w => w.Solved, true);

        var all = await this._wordsDatabase.Find(filter).ToListAsync();

        foreach (var word in all)
        {
            word.Mask = word.Value.Mask();
            word.Solved = false;

            var filter2 = Builders<Word>.Filter.Eq(w => w.Id, word.Id);
            await this._wordsDatabase.ReplaceOneAsync(filter2, word, new ReplaceOptions { IsUpsert = true });
        }

        _logger.LogInformation("all words reset", all.Count);
    }
}

