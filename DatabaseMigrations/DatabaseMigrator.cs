using LutieBot.DataAccess;
using LutieBot.DatabaseMigrations.VersionData;
using SqlKata.Execution;

namespace LutieBot.DatabaseMigrations
{
    public class DatabaseMigrator
    {
        private readonly QueryFactory _db;
        private readonly DatabaseVersionDataProvider _databaseVersionDataProvider;

        private readonly MigrationUtilities _migrationUtilities;

        public DatabaseMigrator(DataAccessMaster dataAccessMaster)
        {
            _db = dataAccessMaster.GetQueryFactory();
            _databaseVersionDataProvider = new DatabaseVersionDataProvider();

            _migrationUtilities = new MigrationUtilities(dataAccessMaster, _databaseVersionDataProvider);
        }

        public async Task MigrateLatest()
        {
            int? currentDatabaseVersion = await _migrationUtilities.GetDatabaseVersion();
            VersionModel latestDatabaseVersionModel = _databaseVersionDataProvider.GetLatestVersion();

            if (currentDatabaseVersion == null)
            {
                Console.WriteLine("Database version not found, assuming is blank database.");

                await CreateDatabase(latestDatabaseVersionModel);

                Console.WriteLine("Database created.");
            }
            else
            {
                Console.WriteLine($"Database version {currentDatabaseVersion} found.");

                VersionModel? currentVersionModel = _databaseVersionDataProvider.GetVersion((int)currentDatabaseVersion);

                if (currentVersionModel == null)
                {
                    throw new Exception($"Version {currentDatabaseVersion} is not a known version!");
                }
                else
                {
                    if (!await _migrationUtilities.ValidateDatabaseVersion(currentVersionModel))
                    {
                        throw new Exception($"Database structure does not match known structure for version {currentDatabaseVersion}!");
                    }

                    if (currentDatabaseVersion < latestDatabaseVersionModel.Version)
                    {
                        Console.WriteLine($"Latest database version is {latestDatabaseVersionModel.Version}, updating database.");

                        await UpdateDatabase(currentVersionModel);

                        Console.WriteLine("Database updated");
                    }
                }
            }
        }

        private async Task CreateDatabase(VersionModel model)
        {
            await _db.StatementAsync(model.CreateScript);
        }

        private async Task UpdateDatabase(VersionModel model)
        {
            IEnumerable<string> updateScripts = _databaseVersionDataProvider.GetUpdateScripts(model.Version);

            foreach (string script in updateScripts)
            {
                await _db.StatementAsync(script);
            }
        }
    }
}
