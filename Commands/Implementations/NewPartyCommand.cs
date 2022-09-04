using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using LutieBot.DataAccess;
using LutieBot.Exceptions;
using LutieBot.Utilities;

namespace LutieBot.Commands.Implementations
{
    public class NewPartyCommand : ApplicationCommandModule
    {
        private readonly EmbedUtilities _embedUtilities;
        private readonly PartyDataAccess _partyDataAccess;

        public NewPartyCommand(EmbedUtilities embedUtilities, PartyDataAccess partyDataAccess)
        {
            _embedUtilities = embedUtilities;
            _partyDataAccess = partyDataAccess;
        }

        [SlashCommand("new-party", "Registers a new party for a specific boss difficulty.")]
        public async Task NewParty(InteractionContext context,
            [Option("name", "The name of the party (must be unique in server).")] string name,
            [Option("members", "Comma-separated IGNs or nicknames of the members to be registered in this party.")] string igns,
            [Option("boss-name", "The name of the boss (e.g. Lucid).")] string? bossName = null,
            [Option("boss-difficulty", "The difficulty of the boss (e.g. Hard)")] string? bossDifficulty = null,
            [Option("boss-abbreviation", "The abbreviation of the boss (e.g. hcid)")] string? bossAbbreviation = null)
        {
            try
            {
                IEnumerable<string> ignList = igns.Split(',').Select(ign => ign.Trim()).Distinct(StringComparer.InvariantCultureIgnoreCase);

                if (!string.IsNullOrEmpty(bossName) && !string.IsNullOrEmpty(bossDifficulty) && string.IsNullOrEmpty(bossAbbreviation))
                {
                    await _partyDataAccess.AddParty(name, ignList, bossName, bossDifficulty, context.Guild.Id);
                }
                else if (string.IsNullOrEmpty(bossName) && string.IsNullOrEmpty(bossDifficulty) && !string.IsNullOrEmpty(bossAbbreviation))
                {
                    await _partyDataAccess.AddParty(name, ignList, bossAbbreviation, context.Guild.Id);
                }
                else
                {
                    await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(_embedUtilities.GetErrorEmbedBuilder("Either a `boss-name` and `boss-difficulty` combination or a `boss-abbreviation` must be provided, and both options cannot be used at the same time!")));
                    return;
                }

                var responseEmbed = _embedUtilities.GetOkEmbedBuilder("Party Added", "The party is successfully added.");
                responseEmbed.AddField("Name", name);

                if (!string.IsNullOrEmpty(bossName))
                {
                    responseEmbed.AddField("Boss", $"{bossName} ({bossDifficulty})");
                }
                else
                {
                    responseEmbed.AddField("Boss", bossAbbreviation);
                }

                responseEmbed.AddField("Members", string.Join(", ", ignList));

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
