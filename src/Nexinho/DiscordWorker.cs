using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using Microsoft.Extensions.Hosting;

namespace Nexinho;

    public class DiscordWorker : BackgroundService
    {
        private readonly DiscordClient _discordClient;

        public DiscordWorker(DiscordClient discordClient)
        {
            _discordClient = discordClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _discordClient.ConnectAsync();
        }
    }
