using DSharpPlus;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using LutieBot.Core;
using LutieBot.Core.ConfigModels;
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
            string prefix = string.Empty;
            DevModeModel? devModeModel = null;

            using IHost host = Host.CreateDefaultBuilder().ConfigureAppConfiguration((context, config) =>
            {
                config.Sources.Clear();
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

                IConfigurationRoot configRoot = config.Build();

                token = configRoot.GetValue<string>("Token");
                prefix = configRoot.GetValue<string>("Prefix");
                devModeModel = configRoot.GetSection("DevMode").Get<DevModeModel>();
            }).Build();

            var lutie = new DiscordClient(new DiscordConfiguration()
            {
                Token = token,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.DirectMessages | DiscordIntents.GuildMessages | DiscordIntents.GuildMessageReactions,
            });

            var commandHandler = new CommandHandler(prefix, devModeModel);
            lutie.MessageCreated += commandHandler.ConsumeCommand;

            lutie.UseInteractivity(new InteractivityConfiguration()
            {
                PollBehaviour = PollBehaviour.KeepEmojis,
                Timeout = TimeSpan.FromSeconds(30)
            });

            await lutie.ConnectAsync();

            await host.RunAsync();
        }
    }
}