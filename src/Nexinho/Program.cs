using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Nexinho.Commands;
using Nexinho.Models;
using Nexinho.Services;

namespace Nexinho;

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
                    services.AddSingleton<WordMongoService>();
                    services.AddSingleton<TriviaMongoService>();
                    services.AddSingleton<RankMongoService>();

                    services.AddHttpClient<IChuckGateway, ChuckGateway>(client =>
                    {
                        client.BaseAddress = new Uri("https://api.chucknorris.io/jokes/random");
                    });

                    services.AddHttpClient<IEvilInsultGateway, EvilInsultGateway>(client =>
                    {
                        client.BaseAddress = new Uri("https://evilinsult.com/generate_insult.php?lang=en&type=json");
                    });

                    services.AddSingleton<IOpenTriviaGateway, OpenTriviaGateway>();

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

                    discord.UseInteractivity(new InteractivityConfiguration
                    {
                        PaginationBehaviour = PaginationBehaviour.Ignore,
                        Timeout = TimeSpan.FromSeconds(30),
                        ButtonBehavior = ButtonPaginationBehavior.DeleteButtons
                    });

                    discord.Ready += Discord_Ready;

                    commands.RegisterCommands<WordGameModule>();
                    commands.RegisterCommands<PhrasesModule>();
                    commands.RegisterCommands<TriviaModule>();

                    services.AddSingleton(discord);

                    services.AddHostedService<DiscordWorker>();
                })
                .ConfigureLogging(logging =>
                {
                    logging.AddConsole();
                })
                ;

        private static Task Discord_Ready(DiscordClient sender, ReadyEventArgs e)
        {
            sender.Logger.LogInformation("Client is ready to process events.");

            return Task.CompletedTask;
        }
    }
