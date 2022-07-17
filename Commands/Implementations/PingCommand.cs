using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using LutieBot.Utilities;

namespace LutieBot.Commands.Implementations
{
    public class PingCommand : ApplicationCommandModule
    {
        private readonly EmbedUtilities _embedUtilities;

        public PingCommand(EmbedUtilities embedUtilities)
        {
            _embedUtilities = embedUtilities;
        }

        [SlashCommand("ping", "Is Lutie alive?")]
        public async Task Ping(InteractionContext context) 
        {
            await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(_embedUtilities.GetOkEmbedBuilder("Pong!", "Lutie received a ping!")));
        }
    }
}