using LutieBot.DataAccess.Models;
using SqlKata.Execution;

namespace LutieBot.DataAccess
{
    public class BossDataAccess
    {
        private readonly QueryFactory _db;

        public BossDataAccess(DataAccessMaster dataAccessMaster)
        {
            _db = dataAccessMaster.GetQueryFactory();
        }

        public async Task<IEnumerable<BossModel>> GetMatchingBosses(string bossAbbreviation)
        {
            var query = _db.Query();

            query.Select("BossDifficulty.Id as Id")
                 .Select("Boss.Name as BossName")
                 .Select("BossDifficulty.Difficulty as BossDifficulty")
                 .Select("BossDifficulty.Abbreviation as BossAbbreviation")
                 .From("BossDifficulty")
                 .LeftJoin("Boss", "Boss.Id", "BossDifficulty.BossId")
                 .WhereLike("BossDifficulty.Abbreviation", bossAbbreviation);

            return await query.GetAsync<BossModel>();
        }

        public async Task<IEnumerable<BossModel>> GetMatchingBosses(string bossName, string bossDifficulty)
        {
            var query = _db.Query();

            query.Select("BossDifficulty.Id as Id")
                 .Select("Boss.Name as BossName")
                 .Select("BossDifficulty.Difficulty as BossDifficulty")
                 .Select("BossDifficulty.Abbreviation as BossAbbreviation")
                 .From("BossDifficulty")
                 .LeftJoin("Boss", "Boss.Id", "BossDifficulty.BossId")
                 .WhereLike("Boss.Name", bossName)
                 .WhereLike("BossDifficulty.Difficulty", bossDifficulty);

            return await query.GetAsync<BossModel>();
        }
    }
}
