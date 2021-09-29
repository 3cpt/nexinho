using System.Threading.Tasks;
using Nexinho.Models;

namespace Nexinho.Services
{
    public interface IChuckGateway
    {
        Task<ChuckJoke> Get();

        Task<ChuckJoke> GetByCategory(string category);
    }
}