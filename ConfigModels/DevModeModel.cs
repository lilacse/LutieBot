namespace LutieBot.ConfigModels
{
    public class DevModeModel
    {
        public bool IsDevMode { get; set; } = false;
        public List<ulong> DiscordServerIds { get; set; } = new List<ulong>();
    }
}
