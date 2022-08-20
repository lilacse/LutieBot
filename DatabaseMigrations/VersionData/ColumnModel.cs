namespace LutieBot.DatabaseMigrations.VersionData
{
    internal record ColumnModel
    {
        public string Name { get; set; } = string.Empty;
        public ColumnType Type { get; set; }
        public bool IsNotNull { get; set; } = true;
        public bool IsPrimaryKey { get; set; } = false;
    }
}
