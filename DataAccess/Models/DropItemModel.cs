namespace LutieBot.DataAccess.Models
{
    public class DropItemModel
    {
        public int Id { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string? ItemAbbreviation { get; set; }
    }
}
