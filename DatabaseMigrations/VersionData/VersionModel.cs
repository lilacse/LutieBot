namespace LutieBot.DatabaseMigrations.VersionData
{
    internal record VersionModel
    {
        public int Version { get; set; }
        public IEnumerable<TableModel> Tables { get; set; } = Enumerable.Empty<TableModel>();
        public string CreateScript { get; set; } = string.Empty;
        public string UpdateScript { get; set; } = string.Empty;
    }
}
