using DSharpPlus;
using DSharpPlus.EventArgs;
using LutieBot.Commands;
using LutieBot.Core.ConfigModels;
using LutieBot.Core.Utilities;
using System.Text;
using System.Text.RegularExpressions;

namespace LutieBot.Core
{
    public class CommandHandler
    {
        private readonly string _prefix;
        private readonly EmbedUtilities _embedUtilities;
        private readonly Dictionary<string, LutieCommand> _commandMap;
        private readonly DevModeModel _devModeModel;

        public CommandHandler(string prefix, DevModeModel? devModeModel = null)
        {
            _prefix = prefix + "lutie ";
            _embedUtilities = new EmbedUtilities();
            _commandMap = new CommandMaster().GetCommandMap();
            _devModeModel = devModeModel ?? new DevModeModel();
        }

        public Task ConsumeCommand(DiscordClient c, MessageCreateEventArgs e)
        {
            if (e.Message.Content.ToLower().StartsWith(_prefix))
            {
                if (_devModeModel.IsDevMode && _devModeModel.DeveloperIds.Contains(e.Author.Id))
                {
                    _ = Task.Run(async () => await DoCommand(c, e));
                }
            }

            return Task.CompletedTask;
        }

        private async Task DoCommand(DiscordClient client, MessageCreateEventArgs messageArgs)
        {
            try
            {
                var tokens = _GetCommandTokens(messageArgs.Message.Content.Substring(_prefix.Length));

                string command = tokens.Dequeue().ToLower();

                if (!_commandMap.ContainsKey(command))
                {
                    await client.SendMessageAsync(messageArgs.Channel, _embedUtilities.GetErrorEmbedBuilder("Command not found!"));
                    return;
                }

                await _commandMap[command].Execute(client, messageArgs, tokens);

            }
            catch (Exception ex)
            {
                await client.SendMessageAsync(messageArgs.Channel, _embedUtilities.GetErrorEmbedBuilder($"An error occured: {ex.Message}"));
            }
        }

        private Queue<string> _GetCommandTokens(string command)
        {
            // Commands in LutieBot follows a pattern similar to Git's: A call to Lutie, followed by an action, then arguments. 
            // e.g. $lutie add-loot hcid "acs armor box"

            var tokens = new Queue<string>();
            var rawCommandTokens = Regex.Split(command, @"([\s""'])");

            bool isInLiteral = false;
            char literalMarker = '\0';
            var currentLiteral = new StringBuilder();
            foreach (string token in rawCommandTokens)
            {
                if (string.IsNullOrWhiteSpace(token))
                {
                    if (isInLiteral)
                    {
                        currentLiteral.Append(token);
                    }
                }
                else if (token == "\"" || token == "'")
                {
                    if (!isInLiteral)
                    {
                        isInLiteral = true;
                        literalMarker = token[0];
                    }
                    else
                    {
                        if (token[0] == literalMarker)
                        {
                            isInLiteral = false;
                            literalMarker = '\0';
                            tokens.Enqueue(currentLiteral.ToString());
                            currentLiteral.Clear();
                        }
                        else
                        {
                            currentLiteral.Append(token);
                        }
                    }
                }
                else
                {
                    if (isInLiteral)
                    {
                        currentLiteral.Append(token);
                    }
                    else
                    {
                        tokens.Enqueue(token);
                    }
                }
            }

            if (isInLiteral) throw new Exception("Ill-formed command: Literal not closed!");

            return tokens;
        }
    }
}
