using System.Threading.Tasks;
using Nexinho.Models;

namespace Nexinho.Services
{
    public interface ITriviaMongoService
    {
        Task Update(Ranking rank);

        Task<Ranking> Get();
    }
}