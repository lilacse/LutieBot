using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using LutieBot.DataAccess;
using LutieBot.Exceptions;
using LutieBot.Utilities;

namespace LutieBot.Commands.Implementations
{
    public class GetPartyCommand : ApplicationCommandModule
    {
        private readonly EmbedUtilities _embedUtilities;
        private readonly PartyDataAccess _partyDataAccess;

        public GetPartyCommand(EmbedUtilities embedUtilities, PartyDataAccess partyDataAccess)
        {
            _embedUtilities = embedUtilities;
            _partyDataAccess = partyDataAccess;
        }

        [SlashCommand("get-party", "Get a list of your parties, optionally filtering by IGN/nickname or bosses.")]
        public async Task GetParty(InteractionContext context,
            [Option("ign", "The IGN or nickname of the member to filter.")] string? ign = null,
            [Option("boss-name", "The name of the boss to filter (e.g. Lucid).")] string? bossName = null,
            [Option("boss-difficulty", "The difficulty of the boss to filter (e.g. Hard)")] string? bossDifficulty = null,
            [Option("boss-abbreviation", "The abbreviation of the boss (e.g. hcid)")] string? bossAbbreviation = null)
        {
            try
            {
                IEnumerable<dynamic> parties = Enumerable.Empty<dynamic>();

                if (!string.IsNullOrEmpty(bossName) && !string.IsNullOrEmpty(bossDifficulty) && string.IsNullOrEmpty(bossAbbreviation))
                {
                    parties = ign == null ? await _partyDataAccess.GetUserParties(context.User.Id, bossName, bossDifficulty, context.Guild.Id) : await _partyDataAccess.GetUserParties(ign, bossName, bossDifficulty, context.Guild.Id);
                }
                else if (string.IsNullOrEmpty(bossName) && string.IsNullOrEmpty(bossDifficulty) && !string.IsNullOrEmpty(bossAbbreviation))
                {
                    parties = ign == null ? await _partyDataAccess.GetUserParties(context.User.Id, bossAbbreviation, context.Guild.Id) : await _partyDataAccess.GetUserParties(ign, bossAbbreviation, context.Guild.Id);
                }
                else if (string.IsNullOrEmpty(bossName) && string.IsNullOrEmpty(bossDifficulty) && string.IsNullOrEmpty(bossAbbreviation))
                {
                    parties = ign == null ? await _partyDataAccess.GetUserParties(context.User.Id, null, null, context.Guild.Id) : await _partyDataAccess.GetUserParties(ign, null, null, context.Guild.Id);
                }
                else
                {
                    await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(_embedUtilities.GetErrorEmbedBuilder("Either a `boss-name` and `boss-difficulty` combination or a `boss-abbreviation` needs to be provided, or none should be provided!")));
                    return;
                }

                var filteredBy = new List<string>();
                if (!string.IsNullOrEmpty(ign))
                {
                    filteredBy.Add($"IGN = {ign}");
                }
                if (!string.IsNullOrEmpty(bossName))
                {
                    filteredBy.Add($"Boss = {bossName} ({bossDifficulty})");
                }
                if (!string.IsNullOrEmpty(bossAbbreviation))
                {
                    filteredBy.Add($"Boss = {bossAbbreviation}");
                }

                var responseEmbed = _embedUtilities.GetOkEmbedBuilder(string.IsNullOrEmpty(ign) ? "Your parties" : "Party List", filteredBy.Any() ? $"Filtered by: {string.Join(", ", filteredBy)}" : string.Empty);

                if (parties.Any())
                {
                    foreach (var party in parties)
                    {
                        responseEmbed.AddField($"{party.PartyName} (id: {party.Id})", $"Boss: {party.BossName} ({party.BossDifficulty})\nMembers: {string.Join(", ", party.Members)}", inline: true);
                    }
                }
                else
                {
                    responseEmbed.AddField("Oops!", "There are no matching parties.");
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
