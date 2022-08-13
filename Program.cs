using DSharpPlus;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using LutieBot.Commands;
using LutieBot.ConfigModels;
using LutieBot.DataAccess;
using LutieBot.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LutieBot
{
    class Program
    {
        static void Main()
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            string token = string.Empty;
            string? connectionString = null;
            DevModeModel? devMode = null;

            using IHost host = Host.CreateDefaultBuilder().ConfigureAppConfiguration((context, config) =>
            {
                config.Sources.Clear();
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

                IConfigurationRoot configRoot = config.Build();

                token = configRoot.GetValue<string>("Token");
                connectionString = configRoot.GetValue<string>("ConnectionString");
                devMode = configRoot.GetSection("DevMode").Get<DevModeModel>();
            }).Build();

            var lutie = new DiscordClient(new DiscordConfiguration()
            {
                Token = token,
                TokenType = TokenType.Bot
            });

            lutie.UseInteractivity(new InteractivityConfiguration()
            {
                PollBehaviour = PollBehaviour.KeepEmojis,
                Timeout = TimeSpan.FromSeconds(30)
            });

            var serviceCollection = new ServiceCollection();
            
            new UtilitiesMaster().RegisterUtilitiesProvider(serviceCollection);

            new DataAccessMaster(connectionString).RegisterDataAccessProviders(serviceCollection);

            new CommandMaster().RegisterSlashCommands(lutie, serviceCollection, devMode);

            await lutie.ConnectAsync();

            await host.RunAsync();
        }
    }
}