using System.Threading.Tasks;
using Nexinho.Models;

namespace Nexinho.Services
{
    public interface IEvilInsultGateway
    {
        Task<EvilInsult> Get();
    }
}
