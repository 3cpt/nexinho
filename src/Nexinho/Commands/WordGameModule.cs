using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using MongoDB.Driver;
using Nexinho.Models;
using Nexinho.Services;

namespace Nexinho.Commands;

public class WordGameModule : BaseCommandModule
{
    public IWordMongoService _wordService { private get; set; }

    public RankMongoService _rankMongo { private get; set; }

    [Command("word")]
    public async Task WordCommand(CommandContext ctx)
    {
        await ctx.TriggerTypingAsync();

        var current = await this._wordService.GetCurrent();

        if (current != null)
        {
            await ctx.RespondAsync($"A palavra é: *{current.Mask}*");
        }
        else
        {
            var word = await this._wordService.GetNext();

            if (word == null)
            {
                await ctx.RespondAsync("Ops, não há mais palavras!");
            }
            else
            {
                word.Current = true;
                word.Mask = word.Value.Mask();
                await this._wordService.UpdateWord(word);
                await ctx.RespondAsync($"A palavra é: {word.Mask}");
            }
        }
    }

    [Command("word")]
    public async Task WordCommand(CommandContext ctx, string word1)
    {
        await ctx.TriggerTypingAsync();

        var current = await _wordService.GetCurrent();

        if (current != null)
        {
            if (current.Value.ToUpper() == word1.ToUpper())
            {
                var emoji = DiscordEmoji.FromName(ctx.Client, ":trophy:");
                var points = current.Mask.Count(f => f == '-');

                await ctx.Message.CreateReactionAsync(emoji);
                await ctx.RespondAsync($"Yes, a palavra era mesmo *{current.Value}* ({points} pontos)");

                current.Current = false;
                current.Solved = true;

                await this._wordService.UpdateWord(current);

                await UpdateRank(ctx.Message.Author.Username, points);
            }
            else if (current.Value == current.Mask)
            {
                current.Current = false;
                current.Solved = true;

                await this._wordService.UpdateWord(current);

                await ctx.RespondAsync($"Não. E já não há mais pistas. A palavra era: {current.Mask}");
            }
            else
            {
                current.Mask = current.Mask.Remask(current.Value);

                await this._wordService.UpdateWord(current);
                await ctx.RespondAsync($"Não. Nova letra: {current.Mask}");
            }
        }
        else
        {
            var word = await this._wordService.GetNext();

            if (word == null)
            {
                await ctx.RespondAsync("Ops, não há mais palavras!");
            }
            else
            {
                word.Current = true;
                await this._wordService.UpdateWord(word);
                await ctx.RespondAsync($"A ultima palavra já foi resolvida. A nova palavra é: {word.Mask}");
            }
        }
    }

    private async Task UpdateRank(string username, int points)
    {
        var ranking = await _rankMongo.GetOrSet(RankCategory.Words);

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

        await _rankMongo.Update(ranking);
    }

    [Command("add")]
    public async Task AddWordCommand(CommandContext ctx, string word)
    {
        await ctx.TriggerTypingAsync();

        var word1 = new Word { Value = word.ToLower(), Mask = word.Mask() };
        var inserted = await this._wordService.InsertWord(word1);

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

