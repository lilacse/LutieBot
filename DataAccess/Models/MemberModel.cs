namespace LutieBot.DataAccess.Models
{
    public class MemberModel
    {
        public int Id { get; set; }
        public int PartyMemberId { get; set; }
        public string Ign { get; set; } = string.Empty;
        public string? Nickname { get; set; }
        public string DiscordId { get; set; } = string.Empty;
        public string DiscordServerId { get; set; } = string.Empty;
    }
}
