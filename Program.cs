using DSharpPlus;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using LutieBot.Commands;
using LutieBot.ConfigModels;
using Microsoft.Extensions.Configuration;
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
            DevModeModel? devMode = null;

            using IHost host = Host.CreateDefaultBuilder().ConfigureAppConfiguration((context, config) =>
            {
                config.Sources.Clear();
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

                IConfigurationRoot configRoot = config.Build();

                token = configRoot.GetValue<string>("Token");
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

            new CommandMaster().RegisterSlashCommands(lutie, devMode);

            await lutie.ConnectAsync();

            await host.RunAsync();
        }
    }
}