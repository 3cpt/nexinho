using System.Collections.Generic;
using System.Threading.Tasks;
using Nexinho.Models;

namespace Nexinho.Gateways;

public interface IOpenTriviaGateway
{
    Task<List<OpenTrivia>> Get(int total = 10, string token = "");

    Task<OpenTriviaToken> GetToken();

    Task<OpenTriviaToken> ResetToken(string currentToken);
}
