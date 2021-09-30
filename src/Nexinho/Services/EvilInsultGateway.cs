using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Nexinho.Models;

namespace Nexinho.Services
{
    public class EvilInsultGateway : IEvilInsultGateway
    {
        private readonly HttpClient httpClient;

        public EvilInsultGateway(HttpClient httpClient)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<EvilInsult> Get()
        {
            try
            {
                var response = await httpClient.GetStringAsync(httpClient.BaseAddress);

                var insult = JsonSerializer.Deserialize<EvilInsult>(response);

                return insult;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                return null;
            }
        }
    }
}
