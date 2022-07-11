using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity.Extensions;
using LutieBot.Commands.Utilities;
using LutieBot.Core.Utilities;
using LutieBot.DataAccess;
using LutieBot.DataAccess.Models;
using LutieBot.Exceptions;

namespace LutieBot.Commands.Implementations
{
    public class AddLoot : LutieCommand
    {
        private readonly EmbedUtilities _embedUtilities;
        private readonly PartyDataAccess _partyDataAccess;
        private readonly DropItemDataAccess _dropItemDataAccess;
        private readonly BossDataAccess _bossDataAccess;
        private readonly CommandUtilities _commandUtilities;

        public AddLoot(EmbedUtilities embedUtilities, PartyDataAccess partyDataAccess, DropItemDataAccess dropItemDataAccess, BossDataAccess bossDataAccess, CommandUtilities commandUtilities)
        {
            _embedUtilities = embedUtilities;
            _partyDataAccess = partyDataAccess;
            _dropItemDataAccess = dropItemDataAccess;
            _bossDataAccess = bossDataAccess;
            _commandUtilities = commandUtilities;
        }

        public async Task Execute(DiscordClient client, MessageCreateEventArgs messageArgs, Queue<string> arguments)
        {
            // This is a work-in-progress feature!

            // Adds loot to a party and add pending claims to the loot. 
            // If full boss name is used, it must follow the syntax of "Difficulty Name", e.g. "Hard Lucid"
            // Syntax:
            // (lutie) add-loot boss loot-name [--quantity quantity] [--party-name party-name] [--exclude "ign-or-nick, ..."] [--include "ign-or-nick, ..."]
            // (lutie) add-loot boss loot-name [--quantity quantity] [--party-id party-id] [--exclude "ign-or-nick, ..."] [--include "ign-or-nick, ..."]

            // TODO: --party-name, --party-id, --include

            if (arguments.Count < 2)
            {
                await client.SendMessageAsync(messageArgs.Channel, _embedUtilities.GetErrorEmbedBuilder("Unexpected number of arguments! (Expecting 2 with optional additionals)"));
                return;
            }

            string bossArg = arguments.Dequeue();
            string itemArg = arguments.Dequeue();

            int quantity = 1;
            string? partyName = null;
            int partyId = -1;
            IEnumerable<string>? excludeList = null;
            IEnumerable<string>? includeList = null;

            try
            {
                GetOptionalArguments(arguments, ref quantity, ref partyName, ref partyId, ref excludeList, ref includeList);
            }
            catch (UserActionException ex)
            {
                await client.SendMessageAsync(messageArgs.Channel, ex.ExceptionMessageEmbed);
                return;
            }
            catch
            {
                throw;
            }

            IEnumerable<BossModel> matchingBosses;

            if (bossArg.Contains(' '))
            {
                string[] bossNameParts = bossArg.Split(' ', 2);
                string bossDifficulty = bossNameParts[0];
                string bossName = bossNameParts[1];

                matchingBosses = await _bossDataAccess.GetMatchingBosses(bossName, bossDifficulty);
            }
            else
            {
                matchingBosses = await _bossDataAccess.GetMatchingBosses(bossArg);
            }

            if (!matchingBosses.Any())
            {
                await client.SendMessageAsync(messageArgs.Channel, _embedUtilities.GetErrorEmbedBuilder($"No bosses match the search criteria '{bossArg}'!"));
                return;
            }
            else if (matchingBosses.Count() > 1)
            {
                var errorEmbed = _embedUtilities.GetErrorEmbedBuilder($"More than one bosses matched! Database might be broken.");
                errorEmbed.AddField("Matching bosses", string.Join("\n", matchingBosses.Select(boss => $"{boss.BossDifficulty} {boss.BossName} ({boss.BossAbbreviation}) (ID: {boss.Id})")));

                await client.SendMessageAsync(messageArgs.Channel, errorEmbed);
                return;
            }

            BossModel boss = matchingBosses.First();

            IEnumerable<PartyModel> userParties = await _partyDataAccess.GetUserParties(messageArgs.Author.Id, messageArgs.Guild.Id);
            IEnumerable<PartyModel> matchingParties = userParties.Where(party => party.BossDifficultyId == boss.Id);

            if (!matchingParties.Any())
            {
                await client.SendMessageAsync(messageArgs.Channel, _embedUtilities.GetErrorEmbedBuilder($"You don't have a party for the boss '{boss.BossDifficulty} {boss.BossName}'!"));
                return;
            }
            else if (matchingParties.Count() > 1)
            {
                var errorEmbed = _embedUtilities.GetErrorEmbedBuilder($"More than one party matched! Please specify one with `--party-name` or `--party-id`.");
                errorEmbed.AddField("Matching parties", string.Join("\n", matchingParties.Select(party => $"{party.PartyName} ({party.BossDifficulty} {party.BossName}) (ID: {party.Id})")));

                await client.SendMessageAsync(messageArgs.Channel, errorEmbed);
                return;
            }

            PartyModel party = matchingParties.First();

            IEnumerable<MemberModel> partyMembers = (await _partyDataAccess.GetPartyMembers(party.Id));
            var excludedMembers = new List<MemberModel>();

            if (excludeList != null)
            {
                IEnumerable<string> _excludeList = excludeList.Select(name => name.ToLower());

                foreach (string excludeIgn in _excludeList)
                {
                    IEnumerable<MemberModel> excludeMatches = partyMembers.Where(member => member.Nickname?.ToLower() == excludeIgn || member.Ign?.ToLower() == excludeIgn);

                    if (!excludeMatches.Any())
                    {
                        await client.SendMessageAsync(messageArgs.Channel, _embedUtilities.GetErrorEmbedBuilder($"Member '{excludeIgn}' not found in party to be excluded!"));
                        return;
                    }
                    else if (excludeMatches.Count() > 1)
                    {
                        await client.SendMessageAsync(messageArgs.Channel, _embedUtilities.GetErrorEmbedBuilder($"Ambiguous exclude member '{excludeIgn}'! Nickname and IGNs of different members should not be the same."));
                        return;
                    }

                    excludedMembers.Add(excludeMatches.First());
                }

                partyMembers = partyMembers.Where(member => !_excludeList.Contains(member.Ign?.ToLower()) && !_excludeList.Contains(member.Nickname?.ToLower()));
            }

            IEnumerable<DropItemModel> matchingItems = await _dropItemDataAccess.GetMatchingDropItems(itemArg);

            if (!matchingItems.Any())
            {
                await client.SendMessageAsync(messageArgs.Channel, _embedUtilities.GetErrorEmbedBuilder($"No items in the database matches '{itemArg}'!"));
                return;
            }
            else if (matchingItems.Count() > 1)
            {
                var errorEmbed = _embedUtilities.GetErrorEmbedBuilder($"More than one items matched! Database might be broken.");
                errorEmbed.AddField("Matching items", string.Join("\n", matchingItems.Select(item => $"{item.ItemName} ({item.ItemAbbreviation}) (ID: {item.Id})")));

                await client.SendMessageAsync(messageArgs.Channel, errorEmbed);
                return;
            }

            DropItemModel item = matchingItems.First();

            var responseEmbed = _embedUtilities.GetInfoEmbedBuilder("Add Item Confirmation", "Are you sure to add this loot record?");
            responseEmbed.AddField("Party", $"{party.PartyName} (ID: {party.Id})");
            responseEmbed.AddField("Members", string.Join(", ", partyMembers.Select(member => member.Ign)));
            if (excludeList != null)
            {
                responseEmbed.AddField("Excludes", string.Join(", ", excludedMembers.Select(member => member.Ign)));
            }
            responseEmbed.AddField("Item", $"{item.ItemName} (x{quantity})");
            responseEmbed.AddField("Boss", $"{party.BossDifficulty} {party.BossName}");

            var responseButtons = new List<DiscordComponent>();
            responseButtons.Add(new DiscordButtonComponent(ButtonStyle.Success, "confirmAddDropItem", "Confirm"));
            responseButtons.Add(new DiscordButtonComponent(ButtonStyle.Danger, "cancelAddDropItem", "Cancel"));

            var responseMessage = new DiscordMessageBuilder().WithEmbed(responseEmbed);
            responseMessage.AddComponents(responseButtons);

            var sentResponseMessage = await client.SendMessageAsync(messageArgs.Channel, responseMessage);
            var userButtonResponse = await sentResponseMessage.WaitForButtonAsync(messageArgs.Author);

            responseMessage.ClearComponents();
            await sentResponseMessage.ModifyAsync(responseMessage);

            if (userButtonResponse.TimedOut)
            {
                await client.SendMessageAsync(messageArgs.Channel, _embedUtilities.GetInfoEmbedBuilder("Action cancelled", $"{messageArgs.Author.Mention}, your request was timed out!"));
            }
            else
            {
                var userInteraction = userButtonResponse.Result.Interaction;

                if (userButtonResponse.Result.Id == "confirmAddDropItem")
                {
                    try
                    {
                        await _dropItemDataAccess.AddDropItem(item.Id, quantity, boss.Id, party.Id, partyMembers);
                        await client.SendMessageAsync(messageArgs.Channel, _embedUtilities.GetOkEmbedBuilder("Add Item Completed", $"{messageArgs.Author.Mention}, the record is successfully added!"));
                    }
                    catch (Exception ex)
                    {
                        await client.SendMessageAsync(messageArgs.Channel, _embedUtilities.GetErrorEmbedBuilder($"{messageArgs.Author.Mention}, something wrong happened while inserting the data. You might want to notify an administrator to clean up the database and investigate the error.\n\nError message: {ex.Message}"));
                    }
                }
                else if (userButtonResponse.Result.Id == "cancelAddDropItem")
                {
                    var cancelResponseEmbed = _embedUtilities.GetInfoEmbedBuilder("Action cancelled", $"{messageArgs.Author.Mention}, your request is cancelled!");
                    await userInteraction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(cancelResponseEmbed));
                }
            }
        }

        private void GetOptionalArguments(Queue<string> arguments, ref int quantity, ref string? partyName, ref int partyId, ref IEnumerable<string>? excludeList, ref IEnumerable<string>? includeList)
        {
            Dictionary<string, string> optionalArguments;
            try
            {
                optionalArguments = _commandUtilities.ParseOptionalArguments(arguments, new List<string> { "quantity", "party-name", "party-id", "exclude", "include" });
            }
            catch
            {
                throw;
            }

            if (optionalArguments.TryGetValue("quantity", out string? quantityString))
            {
                if (!int.TryParse(quantityString, out quantity))
                {
                    var errorEmbed = _embedUtilities.GetErrorEmbedBuilder("Invalid value for argument `--quantity`! (Expecting a number)");
                    throw new UserActionException(errorEmbed);
                }
            }
            optionalArguments.TryGetValue("party-name", out partyName);
            if (optionalArguments.TryGetValue("party-id", out string? partyIdString))
            {
                if (partyName != null)
                {
                    var errorEmbed = _embedUtilities.GetErrorEmbedBuilder("`--party-id` cannot be used together with `--party-name`!");
                    throw new UserActionException(errorEmbed);
                }
                if (!int.TryParse(partyIdString, out partyId))
                {
                    var errorEmbed = _embedUtilities.GetErrorEmbedBuilder("Invalid value for argument `--party-id`! (Expecting a number)");
                    throw new UserActionException(errorEmbed);
                }
            }
            if (optionalArguments.TryGetValue("exclude", out string? excludeString))
            {
                excludeList = excludeString.Split(',').Select(ign => ign.Trim());
            }
            if (optionalArguments.TryGetValue("include", out string? includeString))
            {
                includeList = includeString.Split(',').Select(ign => ign.Trim());
            }
        }
    }
}
