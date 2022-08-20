namespace LutieBot.DatabaseMigrations.VersionData
{
    internal record TableModel
    {
        public string Name { get; set; } = string.Empty;
        public IEnumerable<ColumnModel> Columns = new List<ColumnModel>();
    }
}
