using LutieBot.Exceptions;
using LutieBot.Utilities;
using SqlKata.Execution;

namespace LutieBot.DataAccess
{
    public class MemberDataAccess
    {
        private readonly QueryFactory _db;
        private readonly EmbedUtilities _embedUtilities;

        public MemberDataAccess(DataAccessMaster dataAccessMaster, EmbedUtilities embedUtilities)
        {
            _db = dataAccessMaster.GetQueryFactory();
            _embedUtilities = embedUtilities;
        }

        public async Task<int> GetMemberId(string memberName, ulong discordServerId)
        {
            return await _db.Query("Member")
                            .Select("Member.Id")
                            .WhereLike("Member.Ign", memberName)
                            .OrWhereLike("Member.Nickname", memberName)
                            .Where("Member.DiscordServerId", discordServerId)
                            .FirstOrDefaultAsync<int>();
        }

        public async Task<int> GetMemberId(ulong memberDiscordId, ulong discordServerId)
        {
            return await _db.Query("Member")
                            .Select("Member.Id")
                            .Where("Member.DiscordId", memberDiscordId)
                            .Where("Member.DiscordServerId", discordServerId)
                            .FirstOrDefaultAsync<int>();
        }

        public async Task AddMember(ulong memberDiscordId, string ign, string? nickname, ulong discordServerId)
        {
            if (await GetMemberId(memberDiscordId, discordServerId) != 0)
            {
                throw new UserActionException(_embedUtilities.GetInfoEmbedBuilder("Oops!", "You are already registered!"));
            }

            if (await GetMemberId(ign, discordServerId) != 0)
            {
                throw new UserActionException(_embedUtilities.GetErrorEmbedBuilder($"The IGN \"{ign}\" is already used as an IGN or nickname by another member!"));
            }

            if (!string.IsNullOrEmpty(nickname) && await GetMemberId(nickname, discordServerId) != 0)
            {
                throw new UserActionException(_embedUtilities.GetErrorEmbedBuilder($"The nickname \"{nickname}\" is already used as an IGN or nickname by another member!"));
            }

            var memberQuery = _db.Query("Member");

            await memberQuery.InsertAsync(new 
            {
                Ign = ign, 
                Nickname = nickname, 
                DiscordId = memberDiscordId, 
                DiscordServerId = discordServerId
            });
        }
    }
}