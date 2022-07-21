using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using LutieBot.DataAccess;
using LutieBot.Exceptions;
using LutieBot.Utilities;

namespace LutieBot.Commands.Implementations
{
    public class RegisterCommand : ApplicationCommandModule
    {
        private readonly EmbedUtilities _embedUtilities;
        private readonly MemberDataAccess _memberDataAccess;

        public RegisterCommand(EmbedUtilities embedUtilities, MemberDataAccess memberDataAccess)
        {
            _embedUtilities = embedUtilities;
            _memberDataAccess = memberDataAccess;
        }

        [SlashCommand("register", "Registers you as a party member in this server.")]
        public async Task Register(InteractionContext context, 
            [Option("ign", "Your IGN that people refer you as.")] string ign, 
            [Option("nickname", "Your alternative nickname that people can refer you as.")] string? nickname = null) 
        {
            try
            {
                await _memberDataAccess.AddMember(context.Member.Id, ign, nickname, context.Guild.Id);

                var responseEmbed = _embedUtilities.GetOkEmbedBuilder("Successfully registered", "Your profile in this server is created.");
                responseEmbed.AddField("IGN", ign);
                
                if (!string.IsNullOrEmpty(nickname))
                {
                    responseEmbed.AddField("Nickname", nickname);
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