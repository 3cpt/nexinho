﻿using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Nexinho.Models;

namespace Nexinho.Services
{
    public class ChuckGateway : IChuckGateway
    {
        private readonly HttpClient httpClient;

        public ChuckGateway(HttpClient httpClient)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<ChuckJoke> Get()
        {
            try
            {
                var responseString = await httpClient.GetStringAsync(httpClient.BaseAddress);

                var joke = JsonSerializer.Deserialize<ChuckJoke>(responseString);

                return joke;
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }

        public Task<ChuckJoke> GetByCategory(string category)
        {
            throw new NotImplementedException();
        }
    }
}
