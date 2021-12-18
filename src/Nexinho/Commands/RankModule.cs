using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Nexinho.Models;
using Nexinho.Repositories;
using Nexinho.Extensions;
using System.Collections.Generic;

namespace Nexinho.Commands;

public class RankModule : BaseCommandModule
{
    public RankMongoService _rankMongo { private get; set; }


    [Command("rank")]
    public async Task RankingCommand(CommandContext ctx)
    {
        await ctx.SendMessage("Escreve `rank Words` ou `rank Trivia`");
    }

    [Command("rank")]
    public async Task RankingCommand(CommandContext ctx, string type)
    {
        type = type.Substring(0, 1).ToUpper() + type.Substring(1);

        if (!Enum.TryParse(type, out RankCategory rankCategory))
        {
            await ctx.SendMessage("Escreve `rank Words` ou `rank Trivia`");
        }
        else
        {
            var ranking = await _rankMongo.GetOrSet(rankCategory);

            if (ranking == null)
            {
                await ctx.SendMessage("Ops, não há ranking");
            }
            else
            {
                var sb = new StringBuilder();
                var sortedEmojis = new Dictionary<int, DiscordEmoji>();

                sortedEmojis.Add(0, DiscordEmoji.FromName(ctx.Client, ":first_place:"));
                sortedEmojis.Add(1, DiscordEmoji.FromName(ctx.Client, ":second_place:"));
                sortedEmojis.Add(2, DiscordEmoji.FromName(ctx.Client, ":third_place:"));
                sortedEmojis.Add(3, DiscordEmoji.FromName(ctx.Client, ":thumbsdown:"));

                sb.AppendLine($"{ranking.Id}");

                if (ranking.Ranks == null || ranking.Ranks.Count <= 0)
                {
                    sb.AppendLine($"{DiscordEmoji.FromName(ctx.Client, ":eyes:")} - sem ranking");
                }
                else
                {
                    var sorted = ranking.Ranks.OrderByDescending(o => o.Points).ToList();

                    for (int i = 0; i < sorted.Count; i++)
                    {
                        int currentIndex = 0;
                        if (i > 2)
                        {
                            currentIndex = 3;
                        }
                        else
                        {
                            currentIndex = i;
                        }

                        sb.AppendLine($"{sortedEmojis.GetValueOrDefault(currentIndex)} - {sorted[i].Username} - {sorted[i].Points} pontos");
                    }
                }

                await ctx.SendMessage(sb.ToString());
            }
        }
    }
}
