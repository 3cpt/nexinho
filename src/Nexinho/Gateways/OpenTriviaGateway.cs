using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Nexinho.Models;

namespace Nexinho.Gateways;

public class OpenTriviaGateway : IOpenTriviaGateway
{
    private readonly Uri baseUrl = new Uri("https://opentdb.com/");

    private readonly IHttpClientFactory httpClientFactory;

    public OpenTriviaGateway(IHttpClientFactory httpClientFactory)
    {
        this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
    }

    public async Task<List<OpenTrivia>> Get(int total = 10, string token = "")
    {
        try
        {
            var httpClient = httpClientFactory.CreateClient();

            if (string.IsNullOrEmpty(token))
            {
                httpClient.BaseAddress = new Uri(baseUrl, $"api.php?amount={total}&type=multiple");
            }
            else
            {
                httpClient.BaseAddress = new Uri(baseUrl, $"api.php?amount={total}&type=multiple&token={token}");
            }

            var response = await httpClient.GetStringAsync(httpClient.BaseAddress);

            var questions = JsonSerializer.Deserialize<OpenTriviaResult>(response);

            return questions.Results;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);

            return null;
        }
    }

    public async Task<OpenTriviaToken> GetToken()
    {
        try
        {
            var httpClient = httpClientFactory.CreateClient();

            httpClient.BaseAddress = new Uri(httpClient.BaseAddress, "api_token.php?command=request");

            var response = await httpClient.GetStringAsync(httpClient.BaseAddress);

            var token = JsonSerializer.Deserialize<OpenTriviaToken>(response);

            return token;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);

            return null;
        }
    }

    public async Task<OpenTriviaToken> ResetToken(string currentToken)
    {
        try
        {
            var httpClient = httpClientFactory.CreateClient();

            httpClient.BaseAddress = new Uri(httpClient.BaseAddress, $"api_token.php?command=reset&token={currentToken}");

            var response = await httpClient.GetStringAsync(httpClient.BaseAddress);

            var token = JsonSerializer.Deserialize<OpenTriviaToken>(response);

            return token;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);

            return null;
        }
    }
}
