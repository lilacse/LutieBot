using SqlKata.Execution;

namespace LutieBot.DataAccess
{
    public class MemberDataAccess
    {
        private readonly QueryFactory _db;

        public MemberDataAccess(DataAccessMaster dataAccessMaster)
        {
            _db = dataAccessMaster.GetQueryFactory();
        }

        public async Task RegisterMember(ulong discordId, ulong discordServerId, string ign, string? nickname = null)
        {
            var memberQuery = _db.Query("Member");

            await memberQuery.InsertAsync(new
            {
                Ign = ign,
                Nickname = nickname,
                DiscordId = discordId,
                DiscordServerId = discordServerId
            });
        }
    }
}
