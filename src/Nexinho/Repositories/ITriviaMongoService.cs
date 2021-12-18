using System.Threading.Tasks;
using Nexinho.Models;

namespace Nexinho.Repositories;

public interface ITriviaMongoService
{
    Task Update(Ranking rank);

    Task<Ranking> Get();
}
