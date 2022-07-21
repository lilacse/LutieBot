using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using LutieBot.DataAccess;
using LutieBot.Exceptions;
using LutieBot.Utilities;

namespace LutieBot.Commands.Implementations
{
    public class RegisterMemberCommand : ApplicationCommandModule
    {
        private readonly EmbedUtilities _embedUtilities;
        private readonly MemberDataAccess _memberDataAccess;

        public RegisterMemberCommand(EmbedUtilities embedUtilities, MemberDataAccess memberDataAccess)
        {
            _embedUtilities = embedUtilities;
            _memberDataAccess = memberDataAccess;
        }

        [SlashCommand("register-member", "Registers a server member as a party member in this server.")]
        public async Task RegisterMember(InteractionContext context, 
            [Option("member", "The server member to register.")] DiscordUser user, 
            [Option("ign", "Their IGN that people refer them as.")] string ign, 
            [Option("nickname", "Their alternative nickname that people can refer them as.")] string? nickname = null) 
        {
            try
            {
                var member = (DiscordMember)user;

                await _memberDataAccess.AddMember(member.Id, ign, nickname, context.Guild.Id);

                var responseEmbed = _embedUtilities.GetOkEmbedBuilder("Successfully registered", $"{member.Mention}'s profile in this server is created.");
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