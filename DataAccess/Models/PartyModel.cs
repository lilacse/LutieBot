namespace LutieBot.DataAccess.Models
{
    public class PartyModel
    {
        public int Id { get; set; }
        public string PartyName { get; set; } = string.Empty;
        public string BossName { get; set; } = string.Empty;
        public int BossDifficultyId { get; set; }
        public string BossDifficulty { get; set; } = string.Empty;
    }
}