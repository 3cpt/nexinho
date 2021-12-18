using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using MongoDB.Driver;
using Nexinho.Models;
using Nexinho.Repositories;
using Nexinho.Extensions;

namespace Nexinho.Commands;

public class WordGameModule : BaseCommandModule
{
    private const RankCategory c_Category = RankCategory.Words;

    public WordMongoService _wordService { private get; set; }

    public RankMongoService _rankMongo { private get; set; }

    [Command("word")]
    public async Task WordCommand(CommandContext ctx)
    {
        var current = await _wordService.GetCurrent();

        if (current != null)
        {
            await ctx.SendMessage($"A palavra é: **{current.Mask}**");
        }
        else
        {
            var word = await _wordService.GetNext();

            if (word == null)
            {
                await ctx.SendMessage("Ops, não há mais palavras!");
            }
            else
            {
                word.Current = true;
                word.Mask = word.Value.Mask();
                await this._wordService.UpdateWord(word);
                await ctx.SendMessage($"A palavra é: **{word.Mask}**");
            }
        }
    }

    [Command("word")]
    [Aliases("w")]
    public async Task WordCommand(CommandContext ctx, string word1)
    {
        var current = await _wordService.GetCurrent();

        if (current != null)
        {
            if (current.Value.ToUpper() == word1.ToUpper())
            {
                var emoji = DiscordEmoji.FromName(ctx.Client, ":like_a_sir:");
                var points = current.Mask.Count(f => f == '-');

                await ctx.Message.CreateReactionAsync(emoji);
                await ctx.SendMessage($"Fino, a palavra era mesmo *{current.Value}* ({points} pontos)");

                current.Current = false;
                current.Solved = true;

                await this._wordService.UpdateWord(current);

                await _rankMongo.SetRank(c_Category, ctx.Message.Author.Username, points);
            }
            else if (current.Value == current.Mask)
            {
                current.Current = false;
                current.Solved = true;

                await this._wordService.UpdateWord(current);

                await ctx.SendMessage($"Não. E já não há mais pistas. A palavra era: {current.Mask}");
            }
            else
            {
                current.Mask = current.Mask.Remask(current.Value);

                await this._wordService.UpdateWord(current);
                await ctx.SendMessage($"Nope. Letra adicionada: **{current.Mask}**");
            }
        }
        else
        {
            var word = await this._wordService.GetNext();

            if (word == null)
            {
                await ctx.SendMessage("Ops, não há mais palavras!");
            }
            else
            {
                word.Current = true;
                await this._wordService.UpdateWord(word);
                await ctx.SendMessage($"A ultima palavra já foi resolvida. A nova palavra é: **{word.Mask}**");
            }
        }
    }


    [Command("add")]
    public async Task AddWordCommand(CommandContext ctx, string word)
    {
        var word1 = new Word
        {
            Value = word.ToLower(),
            Mask = word.Mask()
        };

        var inserted = await _wordService.InsertWord(word1);

        if (inserted)
        {
            await ctx.SendMessage($"Palavra adicionada: {word}");
        }
        else
        {
            await ctx.SendMessage("Ops, a palavra já existe");
        }
    }
}

