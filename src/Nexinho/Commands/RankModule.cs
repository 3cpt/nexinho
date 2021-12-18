using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Nexinho.Models;
using Nexinho.Services;

namespace Nexinho.Commands;

public class RankModule : BaseCommandModule
{
    public RankMongoService _rankMongo { private get; set; }


    [Command("rank")]
    public async Task RankingCommand(CommandContext ctx)
    {
        await MessageHelper.SendMessageAsync(ctx, "Escreve `rank Words` ou `rank Trivia`");
    }

    [Command("rank")]
    public async Task RankingCommand(CommandContext ctx, string type)
    {
        if (!Enum.TryParse(type, out RankCategory rankCategory))
        {
            await MessageHelper.SendMessageAsync(ctx, "Escreve `rank Words` ou `rank Trivia`");
        }
        else
        {
            var ranking = await _rankMongo.GetOrSet(rankCategory);

            if (ranking == null)
            {
                await MessageHelper.SendMessageAsync(ctx, "Ops, não há ranking");
            }
            else
            {
                var sb = new StringBuilder();
                var sortedEmojis = new SortedList();

                sortedEmojis.Add(1, DiscordEmoji.FromName(ctx.Client, ":first_place:"));
                sortedEmojis.Add(2, DiscordEmoji.FromName(ctx.Client, ":second_place:"));
                sortedEmojis.Add(3, DiscordEmoji.FromName(ctx.Client, ":third_place:"));
                sortedEmojis.Add(4, DiscordEmoji.FromName(ctx.Client, ":thumbsdown:"));

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

                        sb.AppendLine($"{sortedEmojis[currentIndex]} - {sorted[i].Username} - {sorted[i].Points} pontos");
                    }
                }

                await MessageHelper.SendMessageAsync(ctx, sb.ToString());
            }
        }
    }
}
