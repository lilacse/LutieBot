using LutieBot.DataAccess.Models;
using SqlKata.Execution;

namespace LutieBot.DataAccess
{
    public class PartyDataAccess
    {
        private readonly QueryFactory _db;

        public PartyDataAccess(DataAccessMaster dataAccessMaster)
        {
            _db = dataAccessMaster.GetQueryFactory();
        }

        public async Task<IEnumerable<PartyModel>> GetUserParties(ulong discordId, ulong discordServerId)
        {
            var query = _db.Query();

            query.Select("Party.Id as Id")
                 .Select("Party.Name as PartyName")
                 .Select("Boss.Name as BossName")
                 .Select("BossDifficulty.Id as BossDifficultyId")
                 .Select("BossDifficulty.Difficulty as BossDifficulty")
                 .Select("BossDifficulty.Abbreviation as BossAbbreviation")
                 .From("Member")
                 .LeftJoin("PartyMember", "PartyMember.MemberId", "Member.Id")
                 .LeftJoin("Party", "Party.Id", "PartyMember.PartyId")
                 .LeftJoin("BossDifficulty", "BossDifficulty.Id", "Party.BossDifficultyId")
                 .LeftJoin("Boss", "Boss.Id", "BossDifficulty.BossId")
                 .Where("Member.DiscordId", discordId)
                 .Where("Party.DiscordServerId", discordServerId);

            return await query.GetAsync<PartyModel>();
        }

        public async Task<IEnumerable<MemberModel>> GetPartyMembers(int partyId)
        {
            var query = _db.Query();

            query.Select("Member.Id as Id")
                 .Select("PartyMember.Id as PartyMemberId")
                 .Select("Member.Ign as Ign")
                 .Select("Member.Nickname as Nickname")
                 .Select("Member.DiscordId as DiscordId")
                 .From("Member")
                 .LeftJoin("PartyMember", "PartyMember.MemberId", "Member.Id")
                 .LeftJoin("Party", "Party.Id", "PartyMember.PartyId")
                 .Where("Party.Id", partyId);

            return await query.GetAsync<MemberModel>();
        }

        public async Task AddParty(string partyName, int bossDifficultyId, ulong discordServerId)
        {
            var partyQuery = _db.Query("Party");

            await partyQuery.InsertAsync(new
            {
                Name = partyName,
                BossDifficultyId = bossDifficultyId,
                DiscordServerId = discordServerId
            });
        }

        public async Task AddPartyMember(int partyId, int memberId)
        {
            var partyMemberQuery = _db.Query("PartyMember");

            await partyMemberQuery.InsertAsync(new
            {
                PartyId = partyId, 
                MemberId = memberId
            });
        }

        public async Task RemovePartyMember(int partyId, int memberId)
        {
            var query = _db.Query();

            await query.Where("PartyId", partyId)
                       .Where("MemberId", memberId)
                       .DeleteAsync();
        }
    }
}