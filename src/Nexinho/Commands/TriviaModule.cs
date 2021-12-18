using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using Nexinho.Extensions;
using Nexinho.Gateways;
using Nexinho.Models;
using Nexinho.Repositories;

namespace Nexinho.Commands
{
    public class TriviaModule : BaseCommandModule
    {
        public IOpenTriviaGateway triviaGateway { private get; set; }

        public ITriviaMongoService triviaMongoService { private get; set; }

        private List<OpenTrivia> openTrivias;

        private OpenTrivia currentTrivia;

        [Command("q")]
        public async Task WordCommand(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();

            if (openTrivias == null || openTrivias.Count == 0)
            {
                openTrivias = await triviaGateway.Get();
            }

            if (currentTrivia == null || currentTrivia.Answered)
            {
                if (openTrivias == null || openTrivias.Count == 0)
                    throw new Exception("Can't retrive data from Open Trivia DB");

                currentTrivia = openTrivias[0];
            }

            var interactivity = ctx.Client.GetInteractivity();

            var components = new List<DiscordComponent>();

            var correctButton = string.Empty;

            if (currentTrivia.Type == "multiple")
            {
                var list = new List<string>(currentTrivia.IncorrectAnswers);
                list.Add(currentTrivia.CorrectAnswer);

                list.Shuffle();

                for (int i = 0; i < list.Count; i++)
                {
                    var id = $"id_{i}";
                    if (list[i].GetHashCode() == currentTrivia.CorrectAnswer.GetHashCode())
                    {
                        correctButton = id;
                    }

                    components.Add(new DiscordButtonComponent(ButtonStyle.Primary, id, list[i]));
                }
            }
            else
            {
                components.Add(new DiscordButtonComponent(ButtonStyle.Primary, "id_1", "True"));
                components.Add(new DiscordButtonComponent(ButtonStyle.Secondary, "id_2", "False"));
            }

            var builder = new DiscordMessageBuilder()
                .WithContent(currentTrivia.Question)
                .AddComponents(components);

            var message = await ctx.Client.SendMessageAsync(ctx.Channel, builder);

            var response = await interactivity.WaitForButtonAsync(message, TimeSpan.FromSeconds(20));

            if (!response.TimedOut)
            {
                if (correctButton == response.Result.Id)
                {
                    var points = GetPoints(currentTrivia.Difficulty);

                    var pointsString = points > 1 ? "points" : "point";
                    await ctx.RespondAsync($"wow, it's right. {points} {pointsString} ({currentTrivia.Difficulty}) to {response.Result.Message.Author.Mention}");

                    await UpdateRank(response.Result.User.Username, points);
                }
                else
                {
                    await ctx.RespondAsync($"nope {response.Result.User.Mention}");
                }

                await message.ModifyAsync(new DiscordMessageBuilder()
                    .WithContent($"{currentTrivia.Question} (already answered)"));

                openTrivias.Remove(currentTrivia);
                currentTrivia = null;

            }
            else
            {
                await ctx.RespondAsync("Time over");


                await message.ModifyAsync(new DiscordMessageBuilder()
                    .WithContent($"{currentTrivia.Question} (time over)"));
            }
        }

        private int GetPoints(string difficulty)
        {
            switch (difficulty.ToLower())
            {
                case "easy":
                    return 1;
                case "medium":
                    return 2;
                case "hard":
                    return 3;
                default:
                    return 0;
            }
        }

        private async Task UpdateRank(string username, int points)
        {
            var ranking = await triviaMongoService.Get();

            if (ranking.Ranks.Any(r => r.Username == username))
            {
                ranking.Ranks.First(r => r.Username == username).Points += points;
            }
            else
            {
                ranking.Ranks.Add(new Rank
                {
                    Username = username,
                    Points = points
                });
            }

            await this.triviaMongoService.Update(ranking);
        }
    }
}
