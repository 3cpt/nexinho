using System.Threading.Tasks;
using Nexinho.Models;

namespace Nexinho.Services
{
    public interface IWordService
    {
        Task<Word> GetCurrent();

        Task<Ranking> GetCurrentRanking();

        Task<Word> GetNext();

        Task<bool> InsertWord(Word word);

        Task UpdateCurrentRanking(Ranking rank);

        Task UpdateWord(Word word);
    }
}