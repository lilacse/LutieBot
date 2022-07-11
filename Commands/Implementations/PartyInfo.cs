using DSharpPlus;
using DSharpPlus.EventArgs;
using LutieBot.Core.Utilities;
using LutieBot.DataAccess;
using LutieBot.DataAccess.Models;

namespace LutieBot.Commands.Implementations
{
    public class PartyInfo : LutieCommand
    {
        private readonly EmbedUtilities _embedUtilities;
        private readonly PartyDataAccess _partyDataAccess;

        public PartyInfo(EmbedUtilities embedUtilities, PartyDataAccess partyDataAccess)
        {
            _embedUtilities = embedUtilities;
            _partyDataAccess = partyDataAccess;
        }

        public async Task Execute(DiscordClient client, MessageCreateEventArgs messageArgs, Queue<string> arguments)
        {
            // This is a work-in-progress feature!

            // If used with no arguments, return parties that the user is in. 
            // If used with an argument for IGN, nickname or Discord ID, return parties taht the user is in.
            // If used with an argument for party ID or name, return information for the party. 
            // Syntax: (lutie) party-info ign
            //         (lutie) party-info nickname
            //         (lutie) party-info discord-mention
            //         (lutie) party-info --party party-name
            //         (lutie) party-info --discord-id discord-id
            //         (lutie) party-info --party-id party-id

            if (arguments.Count == 0)
            {
                IEnumerable<PartyModel> partyList = await _partyDataAccess.GetUserParties(messageArgs.Author.Id, messageArgs.Guild.Id);

                if (!partyList.Any())
                {
                    await client.SendMessageAsync(messageArgs.Channel, _embedUtilities.GetInfoEmbedBuilder("No parties found", "You currently are not part of any parties."));
                }
                else
                {
                    await client.SendMessageAsync(messageArgs.Channel, _embedUtilities.GetInfoEmbedBuilder("Your parties", string.Join("\n", partyList.Select(x => $"{x.PartyName} ({x.BossDifficulty} {x.BossName}) (ID: {x.Id})"))));
                }
            }
        }
    }
}