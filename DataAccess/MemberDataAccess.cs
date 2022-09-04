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

        public async Task<ulong> GetMemberDiscordId(string memberName, ulong discordServerId)
        {
            return await _db.Query("Member")
                            .Select("Member.DiscordId")
                            .WhereLike("Member.Ign", memberName)
                            .OrWhereLike("Member.Nickname", memberName)
                            .Where("Member.DiscordServerId", discordServerId)
                            .FirstOrDefaultAsync<ulong>();
        }

        public async Task<int> GetRequiredMemberId(string memberName, ulong discordServerId)
        {
            int memberId = await GetMemberId(memberName, discordServerId);

            if (memberId == 0)
            {
                throw new UserActionException(_embedUtilities.GetErrorEmbedBuilder($"IGN '{memberName}' is not registered by any member in this server!"));
            }

            return memberId;
        }

        public async Task<int> GetRequiredMemberId(ulong memberDiscordId, ulong discordServerId)
        {
            int memberId = await GetMemberId(memberDiscordId, discordServerId);

            if (memberId == 0)
            {
                throw new UserActionException(_embedUtilities.GetErrorEmbedBuilder($"Member with Discord ID {memberDiscordId} is not registered with this bot in this server!"));
            }

            return memberId;
        }

        public async Task<ulong> GetRequiredMemberDiscordId(string memberName, ulong discordServerId)
        {
            ulong memberDiscordId = await GetMemberDiscordId(memberName, discordServerId);

            if (memberDiscordId == 0)
            {
                throw new UserActionException(_embedUtilities.GetErrorEmbedBuilder($"IGN '{memberName}' is not registered by any member in this server!"));
            }

            return memberDiscordId;
        }

        public async Task AddMember(ulong memberDiscordId, string ign, string? nickname, ulong discordServerId)
        {
            if (await GetMemberId(memberDiscordId, discordServerId) != 0)
            {
                throw new UserActionException(_embedUtilities.GetInfoEmbedBuilder("Oops!", "This user is already registered!"));
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