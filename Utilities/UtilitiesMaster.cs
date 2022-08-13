using Microsoft.Extensions.DependencyInjection;

namespace LutieBot.Utilities
{
    public class UtilitiesMaster
    {
        public void RegisterUtilitiesProvider(ServiceCollection collection)
        {
            collection.AddSingleton<EmbedUtilities>();
        }
    }
}