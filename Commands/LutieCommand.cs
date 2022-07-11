using DSharpPlus;
using DSharpPlus.EventArgs;

namespace LutieBot.Commands
{
    public interface LutieCommand
    {
        public Task Execute(DiscordClient client, MessageCreateEventArgs messageArgs, Queue<string> arguments);
    }
}