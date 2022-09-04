using LutieBot.Exceptions;
using LutieBot.Utilities;
using SqlKata.Execution;

namespace LutieBot.DataAccess
{
    public class BossDataAccess
    {
        private readonly QueryFactory _db;
        private readonly EmbedUtilities _embedUtilities;

        public BossDataAccess(DataAccessMaster dataAccessMaster, EmbedUtilities embedUtilities)
        {
            _db = dataAccessMaster.GetQueryFactory();
            _embedUtilities = embedUtilities;
        }

        public async Task<int> GetBossId(string name, ulong discordServerId)
        {
            return await _db.Query("Boss")
                            .Select("Boss.Id")
                            .WhereLike("Boss.Name", name)
                            .Where("Boss.DiscordServerId", discordServerId)
                            .FirstOrDefaultAsync<int>();
        }

        public async Task<int> GetBossDifficultyId(string name, string difficulty, ulong discordServerId)
        {
            return await _db.Query("Boss")
                            .LeftJoin("BossDifficulty", "BossDifficulty.BossId", "Boss.Id")
                            .Select("BossDifficulty.Id")
                            .WhereLike("Boss.Name", name)
                            .WhereLike("BossDifficulty.Difficulty", difficulty)
                            .Where("Boss.DiscordServerId", discordServerId)
                            .FirstOrDefaultAsync<int>();
        }

        public async Task<int> GetBossDifficultyId(int bossId, string difficulty)
        {
            return await _db.Query("BossDifficulty")
                            .Select("BossDifficulty.Id")
                            .Where("BossDifficulty.BossId", bossId)
                            .WhereLike("BossDifficulty.Difficulty", difficulty)
                            .FirstOrDefaultAsync<int>();
        }

        public async Task<int> GetBossDifficultyId(string abbreviation, ulong discordServerId)
        {
            return await _db.Query("Boss")
                            .LeftJoin("BossDifficulty", "BossDifficulty.BossId", "Boss.Id")
                            .LeftJoin("BossDifficultyAbbreviation", "BossDifficultyAbbreviation.BossDifficultyId", "BossDifficulty.Id")
                            .WhereLike("BossDifficultyAbbreviation.Abbreviation", abbreviation)
                            .Where("Boss.DiscordServerId", discordServerId)
                            .FirstOrDefaultAsync<int>();
        }

        public async Task<int> GetRequiredBossDifficultyId(string name, string difficulty, ulong discordServerId)
        {
            if (await GetBossId(name, discordServerId) == 0)
            {
                throw new UserActionException(_embedUtilities.GetErrorEmbedBuilder($"Boss '{name}' is not registered!"));
            }

            int bossDifficultyId = await GetBossDifficultyId(name, difficulty, discordServerId);

            if (bossDifficultyId == 0)
            {
                throw new UserActionException(_embedUtilities.GetErrorEmbedBuilder($"Difficulty '{difficulty}' for boss '{name}' is not registered!"));
            }

            return bossDifficultyId;
        }

        public async Task<int> GetRequiredBossDifficultyId(string abbreviation, ulong discordServerId)
        {
            int bossDifficultyId = await GetBossDifficultyId(abbreviation, discordServerId);

            if (bossDifficultyId == 0)
            {
                throw new UserActionException(_embedUtilities.GetErrorEmbedBuilder($"Boss '{abbreviation}' is not registered!"));
            }

            return bossDifficultyId;
        }

        public async Task<int> InsertBoss(string name, ulong discordServerId)
        {
            var bossQuery = _db.Query("Boss");

            return await bossQuery.InsertGetIdAsync<int>(new
            {
                Name = name,
                DiscordServerId = discordServerId,
            });
        }

        public async Task<int> InsertBossDifficulty(int bossId, string difficulty, IEnumerable<string> abbreviations)
        {
            var bossDifficultyQuery = _db.Query("BossDifficulty");

            int bossDifficultyId = await bossDifficultyQuery.InsertGetIdAsync<int>(new
            {
                BossId = bossId,
                Difficulty = difficulty,
            });

            var bossDifficultyAbbreviationQuery = _db.Query("BossDifficultyAbbreviation");

            foreach (string abbr in abbreviations)
            {
                await bossDifficultyAbbreviationQuery.InsertAsync(new
                {
                    BossDifficultyId = bossDifficultyId,
                    Abbreviation = abbr,
                });
            }

            return bossDifficultyId;
        }

        public async Task AddBoss(string name, string difficulty, IEnumerable<string> abbreviations, ulong discordServerId)
        {
            foreach (string abbr in abbreviations)
            {
                if (await GetBossDifficultyId(abbr, discordServerId) != 0)
                {
                    throw new UserActionException(_embedUtilities.GetErrorEmbedBuilder($"The abbreviation \"{abbr}\" already refers to an existing boss!"));
                }
            }

            int bossId = await GetBossId(name, discordServerId);

            if (bossId != 0)
            {
                if (await GetBossDifficultyId(bossId, difficulty) != 0)
                {
                    throw new UserActionException(_embedUtilities.GetErrorEmbedBuilder($"Boss \"{name}\" already has a difficulty that matches \"{difficulty}\"!"));
                }
                else
                {
                    await InsertBossDifficulty(bossId, difficulty, abbreviations);
                }
            }
            else
            {
                await InsertBossDifficulty(await InsertBoss(name, discordServerId), difficulty, abbreviations);
            }
        }
    }
}