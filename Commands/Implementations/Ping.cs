using DSharpPlus;
using DSharpPlus.EventArgs;
using LutieBot.Core.Utilities;

namespace LutieBot.Commands.Implementations
{
    public class Ping : LutieCommand
    {
        private readonly EmbedUtilities _embedUtilities;

        public Ping(EmbedUtilities embedUtilities)
        {
            _embedUtilities = embedUtilities;
        }

        public async Task Execute(DiscordClient client, MessageCreateEventArgs messageArgs, Queue<string> arguments)
        {
            // Responds to a ping. 
            // Syntax: (lutie) ping

            if (arguments.Count != 0)
            {
                await client.SendMessageAsync(messageArgs.Channel, _embedUtilities.GetErrorEmbedBuilder("Unexpected number of arguments! (Expecting 0)"));
            }
            else
            {
                await client.SendMessageAsync(messageArgs.Channel, _embedUtilities.GetOkEmbedBuilder("Pong!", "Lutie received a ping!"));
            }
        }
    }
}