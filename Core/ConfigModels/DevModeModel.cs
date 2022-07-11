namespace LutieBot.Core.ConfigModels
{
    public class DevModeModel
    {
        public bool IsDevMode { get; set; } = false;
        public List<ulong> DeveloperIds { get; set; } = new List<ulong>();
    }
}
