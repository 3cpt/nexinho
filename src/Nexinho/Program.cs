using System;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using Nexinho.Commands;
using Nexinho.Models;
using Nexinho.Services;

namespace Nexinho
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    // config
                    var botSettings = hostContext.Configuration.GetSection("Bot").Get<BotSettings>();

                    // mongoDb
                    var mongoClient = new MongoClient(botSettings.MongoConnection).GetDatabase(botSettings.DatabaseName);

                    services.AddSingleton(mongoClient);
                    services.AddSingleton<IWordMongoService, WordMongoService>();
                    services.AddSingleton<IChuckGateway, ChuckGateway>();

                    services.AddHttpClient<IChuckGateway, ChuckGateway>(client =>
                    {
                        client.BaseAddress = new Uri("https://api.chucknorris.io/jokes/random");
                    });

                    services.AddHttpClient<IEvilInsultGateway, EvilInsultGateway>(client =>
                    {
                        client.BaseAddress = new Uri("https://evilinsult.com/generate_insult.php?lang=en&type=json");
                    });

                    // dsharpplus
                    var discord = new DiscordClient(new DiscordConfiguration()
                    {
                        Token = botSettings.DiscordToken,
                        TokenType = TokenType.Bot,
                    });

                    var commands = discord.UseCommandsNext(new CommandsNextConfiguration()
                    {
                        StringPrefixes = new[] { botSettings.CommandPrefix },
                        Services = services.BuildServiceProvider()
                    });

                    commands.RegisterCommands<WordGameModule>();
                    commands.RegisterCommands<PhrasesModule>();

                    services.AddSingleton(discord);

                    services.AddHostedService<DiscordWorker>();
                });
    }
}
