using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using Microsoft.Extensions.Hosting;

namespace Nexinho
{
    public class DiscordWorker : BackgroundService
    {
        private readonly DiscordClient discordClient;

        public DiscordWorker(DiscordClient discordClient)
        {
            this.discordClient = discordClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
                await discordClient.ConnectAsync();
        }
    }
}
