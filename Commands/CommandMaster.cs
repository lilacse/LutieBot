using DSharpPlus;
using DSharpPlus.SlashCommands;
using LutieBot.Commands.Implementations;
using LutieBot.ConfigModels;
using Microsoft.Extensions.DependencyInjection;

namespace LutieBot.Commands
{
    public class CommandMaster
    {
        public void RegisterSlashCommands(DiscordClient lutie, ServiceCollection serviceCollection, DevModeModel? devMode)
        {
            var commands = _GetCommandClasses();

            foreach (var command in commands)
            {
                serviceCollection.AddSingleton(command);
            }

            var slashCommands = lutie.UseSlashCommands(new SlashCommandsConfiguration()
            {
                Services = serviceCollection.BuildServiceProvider()
            });

            _RegisterCommands(slashCommands, commands, devMode);
        }

        private IEnumerable<Type> _GetCommandClasses()
        {
            var commandClasses = new List<Type>();

            commandClasses.Add(typeof(PingCommand));
            commandClasses.Add(typeof(NewDropItemCommand));
            commandClasses.Add(typeof(RegisterCommand));
            commandClasses.Add(typeof(RegisterMemberCommand));
            commandClasses.Add(typeof(NewBossCommand));
            commandClasses.Add(typeof(NewPartyCommand));
            commandClasses.Add(typeof(GetPartyCommand));

            return commandClasses;
        }

        private void _RegisterCommands(SlashCommandsExtension slashCommands, IEnumerable<Type> commands, DevModeModel? devMode)
        {
            if (devMode?.IsDevMode == true)
            {
                foreach (var command in commands)
                {
                    foreach (var serverId in devMode.DiscordServerIds)
                    {
                        slashCommands.RegisterCommands(command, serverId);
                    }
                }
            }
            else
            {
                foreach (var command in commands)
                {
                    slashCommands.RegisterCommands(command);
                }
            }
        }
    }
}