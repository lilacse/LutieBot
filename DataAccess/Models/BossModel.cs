namespace LutieBot.DataAccess.Models
{
    public class BossModel
    {
        public int Id { get; set; }
        public string BossName { get; set; } = string.Empty;
        public string BossDifficulty { get; set; } = string.Empty;
        public string? BossAbbreviation { get; set; }
    }
}
