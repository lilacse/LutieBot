using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using SqlKata.Compilers;
using SqlKata.Execution;

namespace LutieBot.DataAccess
{
    public class DataAccessMaster
    {
        private readonly SqliteConnection _connection;
        private readonly SqliteCompiler _compiler;
        private readonly QueryFactory _db;

        public DataAccessMaster(string? connectionString = null)
        {
            _connection = new SqliteConnection(connectionString ?? "Data Source=Lutie.db");
            _compiler = new SqliteCompiler();
            _db = new QueryFactory(_connection, _compiler);
        }

        public SqliteConnection GetSqliteConnection()
        {
            return _connection;
        }

        public SqliteCompiler GetSqliteCompiler()
        {
            return _compiler;
        }

        public QueryFactory GetQueryFactory()
        {
            return _db;
        }

        public void RegisterDataAccessProviders(ServiceCollection collection)
        {
            collection.AddSingleton(typeof(DataAccessMaster), this);

            collection.AddSingleton<DropItemDataAccess>();
            collection.AddSingleton<MemberDataAccess>();
            collection.AddSingleton<BossDataAccess>();
            collection.AddSingleton<PartyDataAccess>();
            collection.AddSingleton<DropDataAccess>();
        }
    }
}