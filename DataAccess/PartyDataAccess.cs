using LutieBot.Exceptions;
using LutieBot.Utilities;
using SqlKata.Execution;

namespace LutieBot.DataAccess
{
    public class PartyDataAccess
    {
        private readonly QueryFactory _db;
        private readonly EmbedUtilities _embedUtilities;
        private readonly BossDataAccess _bossDataAccess;
        private readonly MemberDataAccess _memberDataAccess;

        public PartyDataAccess(DataAccessMaster dataAccessMaster, EmbedUtilities embedUtilities, BossDataAccess bossDataAccess, MemberDataAccess memberDataAccess)
        {
            _db = dataAccessMaster.GetQueryFactory();
            _embedUtilities = embedUtilities;
            _bossDataAccess = bossDataAccess;
            _memberDataAccess = memberDataAccess;
        }

        public async Task<IEnumerable<dynamic>> GetUserParties(ulong discordUserId, int? bossDifficultyId, ulong discordServerId)
        {
            var userPartyCte = _db.Query("Party")
                                  .LeftJoin("PartyMember", "PartyMember.PartyId", "Party.Id")
                                  .LeftJoin("Member", "Member.Id", "PartyMember.MemberId")
                                  .LeftJoin("BossDifficulty", "BossDifficulty.Id", "Party.BossDifficultyId")
                                  .LeftJoin("Boss", "Boss.Id", "BossDifficulty.BossId")
                                  .Select("Party.Id")
                                  .Select("Party.Name as PartyName")
                                  .Select("Boss.Name as BossName")
                                  .Select("BossDifficulty.Difficulty as BossDifficulty")
                                  .Where("Member.DiscordId", discordUserId)
                                  .Where("Party.DiscordServerId", discordServerId);

            if (bossDifficultyId != null)
            {
                userPartyCte.Where("BossDifficulty.Id", bossDifficultyId);
            }

            var result = await _db.Query("UserPartyCte")
                                  .With("UserPartyCte", userPartyCte)
                                  .LeftJoin("PartyMember", "PartyMember.PartyId", "UserPartyCte.Id")
                                  .LeftJoin("Member", "Member.Id", "PartyMember.MemberId")
                                  .Select("UserPartyCte.*")
                                  .Select("Member.Ign")
                                  .GetAsync();

            return result.GroupBy(res => res.Id)
                         .Select(res => new
                         {
                             Id = res.Key,
                             PartyName = res.First().PartyName,
                             BossName = res.First().BossName,
                             BossDifficulty = res.First().BossDifficulty,
                             Members = res.Select(r => r.Ign),
                         });
        }

        public async Task<IEnumerable<dynamic>> GetUserParties(ulong discordUserId, string? bossName, string? bossDifficulty, ulong discordServerId)
        {
            int? bossDifficultyId = null;

            if (!string.IsNullOrEmpty(bossName) && !string.IsNullOrEmpty(bossDifficulty))
            {
                bossDifficultyId = await _bossDataAccess.GetRequiredBossDifficultyId(bossName, bossDifficulty, discordServerId);
            }

            return await GetUserParties(discordUserId, bossDifficultyId, discordServerId);
        }

        public async Task<IEnumerable<dynamic>> GetUserParties(ulong discordUserId, string? bossAbbreviation, ulong discordServerId)
        {
            int? bossDifficultyId = null;

            if (!string.IsNullOrEmpty(bossAbbreviation))
            {
                bossDifficultyId = await _bossDataAccess.GetRequiredBossDifficultyId(bossAbbreviation, discordServerId);
            }

            return await GetUserParties(discordUserId, bossDifficultyId, discordServerId);
        }

        public async Task<IEnumerable<dynamic>> GetUserParties(string ign, string? bossName, string? bossDifficulty, ulong discordServerId)
        {
            int? bossDifficultyId = null;

            if (!string.IsNullOrEmpty(bossName) && !string.IsNullOrEmpty(bossDifficulty))
            {
                bossDifficultyId = await _bossDataAccess.GetRequiredBossDifficultyId(bossName, bossDifficulty, discordServerId);
            }

            ulong discordUserId = await _memberDataAccess.GetRequiredMemberDiscordId(ign, discordServerId);

            return await GetUserParties(discordUserId, bossDifficultyId, discordServerId);
        }

        public async Task<IEnumerable<dynamic>> GetUserParties(string ign, string? bossAbbreviation, ulong discordServerId)
        {
            int? bossDifficultyId = null;

            if (!string.IsNullOrEmpty(bossAbbreviation))
            {
                bossDifficultyId = await _bossDataAccess.GetRequiredBossDifficultyId(bossAbbreviation, discordServerId);
            }

            ulong discordUserId = await _memberDataAccess.GetRequiredMemberDiscordId(ign, discordServerId);

            return await GetUserParties(discordUserId, bossDifficultyId, discordServerId);
        }

        public async Task<int> GetUserPartyId(ulong discordUserId, string bossName, string bossDifficulty, ulong discordServerId)
        {
            IEnumerable<dynamic> userParties = await GetUserParties(discordUserId, bossName, bossDifficulty, discordServerId);

            if (!userParties.Any())
            {
                throw new UserActionException(_embedUtilities.GetErrorEmbedBuilder($"No matching parties found!"));
            }
            else if (userParties.Count() > 1)
            {
                var errorEmbed = _embedUtilities.GetErrorEmbedBuilder($"More than one matching parties found!");
                errorEmbed.AddField("Matching parties", string.Join("\n", userParties.Select(x => $"{x.PartyName} (id: {x.Id})")));

                throw new UserActionException(errorEmbed);
            }

            return Convert.ToInt32(userParties.First().Id);
        }

        public async Task<int> GetUserPartyId(ulong discordUserId, string bossAbbreviation, ulong discordServerId)
        {
            IEnumerable<dynamic> userParties = await GetUserParties(discordUserId, bossAbbreviation, discordServerId);

            if (!userParties.Any())
            {
                throw new UserActionException(_embedUtilities.GetErrorEmbedBuilder($"No matching parties found!"));
            }
            else if (userParties.Count() > 1)
            {
                var errorEmbed = _embedUtilities.GetErrorEmbedBuilder($"More than one matching parties found!");
                errorEmbed.AddField("Matching parties", string.Join("\n", userParties.Select(x => $"{x.PartyName} (id: {x.Id})")));

                throw new UserActionException(errorEmbed);
            }

            return Convert.ToInt32(userParties.First().Id);
        }

        public async Task<int> GetPartyId(string partyName, ulong discordServerId)
        {
            return await _db.Query("Party")
                            .Select("Party.Id")
                            .WhereLike("Party.Name", partyName)
                            .Where("Party.DiscordServerId", discordServerId)
                            .FirstOrDefaultAsync<int>();
        }

        public async Task<dynamic> GetParty(string partyName, ulong discordServerId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<int>> GetMemberIds(string partyName, ulong discordServerId)
        {
            return await _db.Query("PartyMember")
                            .Select("PartyMember.MemberId")
                            .LeftJoin("Party", "Party.Id", "PartyMember.PartyId")
                            .WhereLike("Party.Name", partyName)
                            .Where("Party.DiscordServerId", discordServerId)
                            .GetAsync<int>();
        }

        public async Task<IEnumerable<int>> GetMemberIds(int partyId)
        {
            return await _db.Query("PartyMember")
                            .Select("PartyMember.MemberId")
                            .LeftJoin("Party", "Party.Id", "PartyMember.PartyId")
                            .Where("Party.Id", partyId)
                            .GetAsync<int>();
        }

        public async Task<int> InsertParty(string name, IEnumerable<string> igns, int bossDifficultyId, ulong discordServerId)
        {
            if (await GetPartyId(name, discordServerId) != 0)
            {
                throw new UserActionException(_embedUtilities.GetErrorEmbedBuilder($"Party name '{name}' is already used!"));
            }

            List<int> partyMemberIds = new();

            foreach (string ign in igns)
            {
                int partyMemberId = await _memberDataAccess.GetRequiredMemberId(ign, discordServerId);

                partyMemberIds.Add(partyMemberId);
            }

            var partyQuery = _db.Query("Party");

            int partyId = await partyQuery.InsertGetIdAsync<int>(new
            {
                Name = name,
                BossDifficultyId = bossDifficultyId,
                DiscordServerId = discordServerId,
            });

            var partyMemberQuery = _db.Query("PartyMember");

            foreach (int partyMemberId in partyMemberIds)
            {
                await partyMemberQuery.InsertAsync(new
                {
                    PartyId = partyId,
                    MemberId = partyMemberId,
                });
            }

            return partyId;
        }

        public async Task<int> AddParty(string name, IEnumerable<string> igns, string bossName, string bossDifficulty, ulong discordServerId)
        {
            int bossDifficultyId = await _bossDataAccess.GetRequiredBossDifficultyId(bossName, bossDifficulty, discordServerId);

            int partyId = await InsertParty(name, igns, bossDifficultyId, discordServerId);

            return partyId;
        }

        public async Task<int> AddParty(string name, IEnumerable<string> igns, string bossAbbreviation, ulong discordServerId)
        {
            int bossDifficultyId = await _bossDataAccess.GetRequiredBossDifficultyId(bossAbbreviation, discordServerId);

            int partyId = await InsertParty(name, igns, bossDifficultyId, discordServerId);

            return partyId;
        }

        public async Task<int> AddMembersToParty(string partyName, IEnumerable<string> igns, ulong discordServerId)
        {
            throw new NotImplementedException();
        }

        public async Task<int> RemoveMembersFromParty(string partyName, IEnumerable<string> igns, ulong discordServerId)
        {
            throw new NotImplementedException();
        }
    }
}
