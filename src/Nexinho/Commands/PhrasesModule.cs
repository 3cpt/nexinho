using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Nexinho.Services;

namespace Nexinho.Commands
{
    public class PhrasesModule : BaseCommandModule
    {
        public IChuckGateway chuckGateway { private get; set; }

        [Command("chuck")]
        public async Task ChuckCommand(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();

            var joke = await chuckGateway.Get();

            await ctx.RespondAsync(joke.Value);
        }
    }
}

