using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Nexinho.Services;

namespace Nexinho.Commands
{
    public class PhrasesModule : BaseCommandModule
    {
        public IChuckGateway chuckGateway { private get; set; }

        public IEvilInsultGateway evilInsultGateway { private get; set; }

        [Command("chuck")]
        public async Task ChuckCommand(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();

            var joke = await chuckGateway.Get();

            if (joke == default)
            {
                await ctx.RespondAsync("Chuck is resting");
            }
            else
            {
                await ctx.RespondAsync(joke.Value);
            }
        }

        [Command("insult")]
        public async Task InsultCommand(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();

            var insult = await evilInsultGateway.Get();

            if (insult == default)
            {
                await ctx.RespondAsync("You shouldn't insult anyone...");
            }
            else
            {
                await ctx.RespondAsync(insult.Insult);
            }
        }
    }
}
