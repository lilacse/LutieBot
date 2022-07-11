using DSharpPlus.Entities;

namespace LutieBot.Exceptions
{
    public class UserActionException : Exception
    {
        public UserActionException(DiscordEmbed? discordEmbed)
        {
            ExceptionMessageEmbed = discordEmbed;
        }

        public DiscordEmbed? ExceptionMessageEmbed { get; set; }
    }
}
