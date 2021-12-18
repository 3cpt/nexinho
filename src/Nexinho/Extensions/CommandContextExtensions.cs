using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

namespace Nexinho.Extensions;

public static class CommandContextExtensions
{
    public static async Task<DiscordMessage> SendMessage(this CommandContext ctx, string message)
    {
        await ctx.TriggerTypingAsync();

        var builder = new DiscordMessageBuilder()
            .WithContent(message);

        return await ctx.Client.SendMessageAsync(ctx.Channel, builder);
    }
}
