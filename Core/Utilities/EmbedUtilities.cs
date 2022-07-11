using DSharpPlus.Entities;

namespace LutieBot.Core.Utilities
{
    public class EmbedUtilities
    {
        public DiscordEmbedBuilder GetErrorEmbedBuilder(string errorMessage)
        {
            return new DiscordEmbedBuilder
            {
                Color = DiscordColor.Red,
                Title = "Oops!",
                Description = errorMessage
            };
        }


        public DiscordEmbedBuilder GetOkEmbedBuilder(string title, string message)
        {
            return new DiscordEmbedBuilder
            {
                Color = DiscordColor.Green,
                Title = title,
                Description = message
            };
        }

        public DiscordEmbedBuilder GetInfoEmbedBuilder(string title, string message)
        {
            return new DiscordEmbedBuilder
            {
                Color = DiscordColor.Blue,
                Title = title,
                Description = message
            };
        }
    }
}