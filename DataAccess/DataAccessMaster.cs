using Microsoft.Data.Sqlite;
using SqlKata.Compilers;
using SqlKata.Execution;

namespace LutieBot.DataAccess
{
    public class DataAccessMaster
    {
        private readonly SqliteConnection _connection;
        private readonly SqliteCompiler _compiler;
        private readonly QueryFactory _db;

        public DataAccessMaster()
        {
            _connection = new SqliteConnection("Data Source=Lutie.db");
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
    }
}