using LutieBot.DataAccess;
using LutieBot.DatabaseMigrations.VersionData;
using SqlKata.Execution;

namespace LutieBot.DatabaseMigrations
{
    internal class MigrationUtilities
    {
        private readonly QueryFactory _db;
        private readonly DatabaseVersionDataProvider _databaseVersionDataProvider;

        public MigrationUtilities(DataAccessMaster dataAccessMaster, DatabaseVersionDataProvider databaseVersionDataProvider)
        {
            _db = dataAccessMaster.GetQueryFactory();
            _databaseVersionDataProvider = databaseVersionDataProvider;
        }

        public async Task<int?> GetDatabaseVersion()
        {
            bool versionColumnExists = (await _db.SelectAsync(@"select name as Name from pragma_table_info(""_Version"") where name = 'Version'")).Any();

            if (versionColumnExists)
            {
                return await _db.Query("_Version")
                                .Select("Version")
                                .FirstOrDefaultAsync<int?>();
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> ValidateDatabaseVersion(VersionModel version)
        {
            foreach (TableModel table in version.Tables)
            {
                IEnumerable<ColumnModel> columns = await _db.SelectAsync<ColumnModel>(@$"select name as Name, ""type"" as ""Type"", ""notnull"" as IsNotNull, pk as IsPrimaryKey from pragma_table_info(""{table.Name}"")");

                if (table.Columns.Intersect(columns).Count() != columns.Count())
                {
                    return false;
                }
            }

            return true;
        }
    }
}
