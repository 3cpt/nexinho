using System.Threading.Tasks;
using Nexinho.Models;

namespace Nexinho.Gateways;
public interface IEvilInsultGateway
{
    Task<EvilInsult> Get();
}
