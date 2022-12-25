using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using LutieBot.DataAccess;
using LutieBot.Exceptions;
using LutieBot.Utilities;

namespace LutieBot.Commands.Implementations
{
    public class AddDropCommand : ApplicationCommandModule
    {
        private readonly EmbedUtilities _embedUtilities;
        private readonly DropDataAccess _dropDataAccess;

        public AddDropCommand(EmbedUtilities embedUtilities, DropDataAccess dropDataAccess)
        {
            _embedUtilities = embedUtilities;
            _dropDataAccess = dropDataAccess;
        }

        [SlashCommand("add-drop", "Adds a new drop for a boss run.")]
        public async Task AddDrop(InteractionContext context,
            [Option("item", "The name or abbreviation of the item (e.g. Purple Cube).")] string item,
            [Option("boss-name", "The name of the boss (e.g. Lucid).")] string? bossName = null,
            [Option("boss-difficulty", "The difficulty of the boss")] string? bossDifficulty = null,
            [Option("boss-abbreviation", "The abbreviation of the boss (e.g. hcid)")] string? bossAbbreviation = null,
            [Option("party-name", "The abbreviation of the boss (e.g. hcid)")] string? partyName = null,
            [Option("exclude", "Comma-separated list of members from the party to exclude from this drop")] string? excludes = null)
        {
            try
            {
                IEnumerable<string> excludeList = excludes?.Split(',').Select(memberName => memberName.Trim()) ?? Enumerable.Empty<string>();

                if (!string.IsNullOrEmpty(bossName) && !string.IsNullOrEmpty(bossDifficulty) && string.IsNullOrEmpty(bossAbbreviation) && string.IsNullOrEmpty(partyName))
                {
                    await _dropDataAccess.AddDrop(item, bossName, bossDifficulty, context.Guild.Id, context.Member.Id, excludeList);
                }
                else if (string.IsNullOrEmpty(bossName) && string.IsNullOrEmpty(bossDifficulty) && !string.IsNullOrEmpty(bossAbbreviation) && string.IsNullOrEmpty(partyName))
                {
                    await _dropDataAccess.AddDrop(item, bossAbbreviation, context.Guild.Id, context.Member.Id, excludeList);
                }
                else if (string.IsNullOrEmpty(bossName) && string.IsNullOrEmpty(bossDifficulty) && string.IsNullOrEmpty(bossAbbreviation) && !string.IsNullOrEmpty(partyName))
                {
                    await _dropDataAccess.AddDrop(item, partyName, context.Guild.Id, excludeList);
                }
                else
                {
                    await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(_embedUtilities.GetErrorEmbedBuilder("Either a `boss-name` and `boss-difficulty` combination or `boss-abbreviation` or `party-name` must be provided, and the options cannot be used at the same time!")));
                    return;
                }

                var responseEmbed = _embedUtilities.GetOkEmbedBuilder("Drop Added", "The drop is successfully added.");
                responseEmbed.AddField("Item", item);

                // TODO: Make data access methods return results and create responses based on those results instead.
                if (!string.IsNullOrEmpty(bossName))
                {
                    responseEmbed.AddField("Boss Name", bossName);
                }
                if (!string.IsNullOrEmpty(bossDifficulty))
                {
                    responseEmbed.AddField("Boss Difficulty", bossDifficulty);
                }
                if (!string.IsNullOrEmpty(bossAbbreviation))
                {
                    responseEmbed.AddField("Boss", bossAbbreviation);
                }
                if (!string.IsNullOrEmpty(partyName))
                {
                    responseEmbed.AddField("Party Name", partyName);
                }
                if (!string.IsNullOrEmpty(excludes))
                {
                    responseEmbed.AddField("Excluded", string.Join(", ", excludeList));
                }

                await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(responseEmbed));
            }
            catch (UserActionException ex)
            {
                await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(ex.ExceptionMessageEmbed));
                return;
            }
            catch (Exception ex)
            {
                await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(_embedUtilities.GetErrorEmbedBuilder($"An error occured: {ex.Message}")));
                return;
            }
        }
    }
}
