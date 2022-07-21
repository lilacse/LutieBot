using DSharpPlus;
using DSharpPlus.SlashCommands;
using LutieBot.Commands.Implementations;
using LutieBot.ConfigModels;
using LutieBot.Utilities;
using LutieBot.DataAccess;
using Microsoft.Extensions.DependencyInjection;

namespace LutieBot.Commands
{
    public class CommandMaster
    {
        public void RegisterSlashCommands(DiscordClient lutie, DevModeModel? devMode)
        {
            var commands = _GetCommandClasses();

            var slashCommands = lutie.UseSlashCommands(new SlashCommandsConfiguration()
            {
                Services = _GetCommandsServiceProvider(commands)
            });

            _RegisterCommands(slashCommands, commands, devMode);
        }

        private IEnumerable<Type> _GetCommandClasses()
        {
            var commandClasses = new List<Type>();

            commandClasses.Add(typeof(PingCommand));
            commandClasses.Add(typeof(NewDropItemCommand));
            commandClasses.Add(typeof(RegisterCommand));

            return commandClasses;
        }

        private ServiceProvider _GetCommandsServiceProvider(IEnumerable<Type> commands)
        {
            var commandsCollection = new ServiceCollection();

            // commands
            foreach (var command in commands)
            {
                commandsCollection.AddSingleton(command);
            }

            // dependencies - message generating
            commandsCollection.AddSingleton<EmbedUtilities>();

            // dependencies - data access
            commandsCollection.AddSingleton<DataAccessMaster>();
            commandsCollection.AddSingleton<DropItemDataAccess>();
            commandsCollection.AddSingleton<MemberDataAccess>();

            return commandsCollection.BuildServiceProvider();
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