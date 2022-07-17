using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using LutieBot.DataAccess;
using LutieBot.Exceptions;
using LutieBot.Utilities;

namespace LutieBot.Commands.Implementations
{
    public class NewDropItemCommand : ApplicationCommandModule
    {
        private readonly EmbedUtilities _embedUtilities;
        private readonly DropItemDataAccess _dropItemDataAccess;

        public NewDropItemCommand(EmbedUtilities embedUtilities, DropItemDataAccess dropItemDataAccess)
        {
            _embedUtilities = embedUtilities;
            _dropItemDataAccess = dropItemDataAccess;
        }

        [SlashCommand("new-drop-item", "Registers a new drop item.")]
        public async Task NewDropItem(InteractionContext context,
            [Option("item-name", "The name of the item.")] string itemName,
            [Option("abbreviations", "Comma-separated list of abbreviations for the item.")] string? abbreviations = null)
        {
            try
            {
                IEnumerable<string> abbreviationList = abbreviations == null ? Enumerable.Empty<string>() : abbreviations.Split(',').Select(abbr => abbr.Trim().ToLower());

                await _dropItemDataAccess.AddDropItem(itemName, abbreviationList, context.Guild.Id);

                var responseEmbed = _embedUtilities.GetOkEmbedBuilder("Item Added", "The item is successfully added.");
                responseEmbed.AddField("Item name", itemName);
                
                if (abbreviationList.Any())
                {
                    responseEmbed.AddField("Abbreviations", string.Join(", ", abbreviationList));
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
