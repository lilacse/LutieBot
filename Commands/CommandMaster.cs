using LutieBot.Commands.Implementations;
using LutieBot.Commands.Utilities;
using LutieBot.Core.Utilities;
using LutieBot.DataAccess;
using Microsoft.Extensions.DependencyInjection;

namespace LutieBot.Commands
{
    public class CommandMaster
    {
        private Dictionary<string, LutieCommand> _CommandMap;

        public CommandMaster()
        {
            _CommandMap = new Dictionary<string, LutieCommand>();

            var commandsCollection = new ServiceCollection();
            
            // dependencies - message generating
            commandsCollection.AddSingleton<EmbedUtilities>();

            // dependencies - command handling
            commandsCollection.AddSingleton<CommandUtilities>();

            // dependencies - data access
            commandsCollection.AddSingleton<DataAccessMaster>();
            commandsCollection.AddSingleton<PartyDataAccess>();
            commandsCollection.AddSingleton<DropItemDataAccess>();
            commandsCollection.AddSingleton<BossDataAccess>();

            // commands
            commandsCollection.AddSingleton<Ping>();
            commandsCollection.AddSingleton<PartyInfo>();
            commandsCollection.AddSingleton<AddLoot>();

            var commandsProvider = commandsCollection.BuildServiceProvider();

            // add commands into the map
            _CommandMap.Add("ping", commandsProvider.GetRequiredService<Ping>());
            _CommandMap.Add("party-info", commandsProvider.GetRequiredService<PartyInfo>());
            _CommandMap.Add("add-loot", commandsProvider.GetRequiredService<AddLoot>());
        }

        public Dictionary<string, LutieCommand> GetCommandMap()
        {
            return _CommandMap;
        }
    }
}