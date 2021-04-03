using System;
using System.Linq;
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
                    await ctx.RespondAsync($"Parabéns, a palavra era {current.Value}");

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
                else
                {
                    current.Mask = current.Mask.Remask(current.Value);

                    await this.wordService.UpdateWord(current);
                    await ctx.RespondAsync($"Parabéns, a palavra era {current.Mask}");
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
    }
}
