using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

namespace Nexinho.Commands;
public class MessageHelper
{
    public static async Task<DiscordMessage> SendMessageAsync(CommandContext ctx, string message)
    {
        await ctx.TriggerTypingAsync();

        var builder = new DiscordMessageBuilder()
            .WithContent(message);

        return await ctx.Client.SendMessageAsync(ctx.Channel, builder);
    }
}
