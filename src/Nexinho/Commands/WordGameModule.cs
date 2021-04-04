using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using MongoDB.Driver;
using Nexinho.Models;
using Nexinho.Services;

namespace Nexinho.Commands
{
    public class WordGameModule : BaseCommandModule
    {
        public IWordService wordService { private get; set; }

        [Command("word")]
        public async Task WordCommand(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();

            var current = await this.wordService.GetCurrent();

            if (current != null)
            {
                await ctx.RespondAsync($"A palavra é: {current.Mask}");
            }
            else
            {
                var word = await this.wordService.GetNext();

                if (word == null)
                {
                    await ctx.RespondAsync("Ops, não há mais palavras!");
                }
                else
                {
                    word.Current = true;
                    word.Mask = word.Value.Mask();
                    await this.wordService.UpdateWord(word);
                    await ctx.RespondAsync($"A palavra é: {word.Mask}");
                }
            }
        }

        [Command("word")]
        public async Task WordCommand(CommandContext ctx, string word1)
        {
            await ctx.TriggerTypingAsync();

            var current = await this.wordService.GetCurrent();

            if (current != null)
            {
                if (current.Value.ToUpper() == word1.ToUpper())
                {
                    var emoji = DiscordEmoji.FromName(ctx.Client, ":trophy:");

                    await ctx.Message.CreateReactionAsync(emoji);
                    await ctx.RespondAsync($"Parabéns, a palavra era mesmo {current.Value}");

                    current.Current = false;
                    current.Solved = true;

                    await this.wordService.UpdateWord(current);

                    var ranking = await this.wordService.GetCurrentRanking();

                    if (ranking.Ranks.Any(r => r.Username == ctx.Message.Author.Username))
                    {
                        ranking.Ranks.First(r => r.Username == ctx.Message.Author.Username).Points
                            = current.Mask.Count(f => f == '-');
                    }
                    else
                    {
                        ranking.Ranks.Add(new Rank
                        {
                            Username = ctx.Message.Author.Username,
                            Points
                            = current.Mask.Count(f => f == '-')
                        });
                    }

                    await this.wordService.UpdateCurrentRanking(ranking);
                }
                else if (current.Value == current.Mask)
                {
                    current.Current = false;
                    current.Solved = true;

                    await this.wordService.UpdateWord(current);

                    await ctx.RespondAsync($"Não. E já não há mais pistas. A palavra era: {current.Mask}");
                }
                else
                {
                    current.Mask = current.Mask.Remask(current.Value);

                    await this.wordService.UpdateWord(current);
                    await ctx.RespondAsync($"Não. Nova letra: {current.Mask}");
                }
            }
            else
            {
                var word = await this.wordService.GetNext();

                if (word == null)
                {
                    await ctx.RespondAsync("Ops, não há mais palavras!");
                }
                else
                {
                    word.Current = true;
                    await this.wordService.UpdateWord(word);
                    await ctx.RespondAsync($"A ultima palavra já foi resolvida. A nova palavra é: {word.Mask}");
                }
            }
        }

        [Command("add")]
        public async Task AddWordCommand(CommandContext ctx, string word)
        {
            await ctx.TriggerTypingAsync();

            var word1 = new Word { Value = word.ToLower(), Mask = word.Mask() };
            var inserted = await this.wordService.InsertWord(word1);

            if (inserted)
            {
                await ctx.RespondAsync($"Palavra adicionada: {word}");
            }
            else
            {
                await ctx.RespondAsync("Ops, a palavra já existe");
            }
        }

        [Command("ranking")]
        public async Task RankingCommand(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();

            var emoji1st = DiscordEmoji.FromName(ctx.Client, ":first_place:");
            var emoji2nd = DiscordEmoji.FromName(ctx.Client, ":second_place:");
            var emoji3rd = DiscordEmoji.FromName(ctx.Client, ":third_place:");


            var ranking = await this.wordService.GetCurrentRanking();

            if (ranking != null)
            {
                var sb = new StringBuilder();

                sb.Append($"Ranking - {ranking.Id}:");

                if (ranking.Ranks == null || ranking.Ranks.Count <= 0)
                {
                    sb.AppendLine($"{emoji1st} - ninguém");
                }
                else
                {
                    var sorted = ranking.Ranks.OrderByDescending(o => o.Points).ToList();

                    sb.AppendLine($"{emoji1st} - {sorted[0].Username} - {sorted[0].Points} pontos");

                    if (sorted.Count > 1)
                    {
                        sb.AppendLine($"{emoji1st} - {sorted[1].Username} - {sorted[1].Points} pontos");

                        if (sorted.Count > 2)
                        {
                            sb.AppendLine($"{emoji1st} - {sorted[2].Username} - {sorted[2].Points} pontos");
                        }
                    }
                }

                await ctx.RespondAsync(sb.ToString());
            }
            else
            {
                await ctx.RespondAsync("Ops, não há ranking");
            }
        }
    }
}
