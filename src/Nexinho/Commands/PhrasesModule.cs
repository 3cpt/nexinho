using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
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

        [Command("pepe"), Aliases("feelsbadman"), Description("Feels bad, man.")]
        public async Task Pepe(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();

            // wrap it into an embed
            var embed = new DiscordEmbedBuilder
            {
                Title = "Pepe",
                ImageUrl = "http://i.imgur.com/44SoSqS.jpg"
            };

            await ctx.RespondAsync(embed);
        }
    }
}
