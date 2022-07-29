using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using LutieBot.DataAccess;
using LutieBot.Exceptions;
using LutieBot.Utilities;

namespace LutieBot.Commands.Implementations
{
    public class NewBossCommand : ApplicationCommandModule
    {
        private readonly EmbedUtilities _embedUtilities;
        private readonly BossDataAccess _bossDataAccess;

        public NewBossCommand(EmbedUtilities embedUtilities, BossDataAccess bossDataAccess)
        {
            _embedUtilities = embedUtilities;
            _bossDataAccess = bossDataAccess;
        }

        [SlashCommand("new-boss", "Registers a new boss.")]
        public async Task NewBoss(InteractionContext context, 
            [Option("name", "The name of the boss (e.g. Lucid).")] string name,
            [Option("difficulty", "The difficulty the boss (e.g. Hard).")] string difficulty,
            [Option("abbreviations", "Comma-separated list of abbreviations for this boss and difficulty (e.g. hcid, hluc).")] string? abbreviations = null) 
        {
            try
            {
                IEnumerable<string> abbreviationList = abbreviations == null ? Enumerable.Empty<string>() : abbreviations.Split(',').Select(abbr => abbr.Trim().ToLower()).Distinct();

                await _bossDataAccess.AddBoss(name, difficulty, abbreviationList, context.Guild.Id);

                var responseEmbed = _embedUtilities.GetOkEmbedBuilder("Boss Added", "The boss is successfully added.");
                responseEmbed.AddField("Name", name);
                responseEmbed.AddField("Difficulty", difficulty);
                
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