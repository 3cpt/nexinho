using System.Threading.Tasks;
using Nexinho.Models;

namespace Nexinho.Repositories;

public interface IWordMongoService
{
    Task<Word> GetCurrent();

    Task<Word> GetNext();

    Task<bool> InsertWord(Word word);

    Task UpdateWord(Word word);

    Task Reset();
}