using LutieBot.Exceptions;
using LutieBot.Utilities;
using SqlKata.Execution;

namespace LutieBot.DataAccess
{
    public class DropDataAccess
    {
        private readonly QueryFactory _db;
        private readonly EmbedUtilities _embedUtilities;
        private readonly DropItemDataAccess _dropItemDataAccess;
        private readonly PartyDataAccess _partyDataAccess;
        private readonly MemberDataAccess _memberDataAccess;

        public DropDataAccess(DataAccessMaster dataAccessMaster, EmbedUtilities embedUtilities, DropItemDataAccess dropItemDataAccess, PartyDataAccess partyDataAccess, MemberDataAccess memberDataAccess)
        {
            _db = dataAccessMaster.GetQueryFactory();
            _embedUtilities = embedUtilities;
            _dropItemDataAccess = dropItemDataAccess;
            _partyDataAccess = partyDataAccess;
            _memberDataAccess = memberDataAccess;
        }

        public async Task<int> InsertPartyDropRecord(int itemId, int partyId, IEnumerable<int> memberIds)
        {
            var partyDropRecordQuery = _db.Query("PartyDropRecord");

            int partyDropRecordId = await partyDropRecordQuery.InsertGetIdAsync<int>(new
            {
                ItemId = itemId,
                PartyId = partyId,
                Timestamp = DateTime.UtcNow,
            });

            foreach (int memberId in memberIds)
            {
                await InsertPartyDropSplit(partyDropRecordId, memberId);
            }

            return partyDropRecordId;
        }

        public async Task<int> InsertPartyDropSplit(int partyDropRecordId, int memberId, decimal? splitAmount = null)
        {
            var partyDropRecordQuery = _db.Query("PartyDropSplit");

            return await partyDropRecordQuery.InsertGetIdAsync<int>(new
            {
                PartyDropRecordId = partyDropRecordId,
                MemberId = memberId,
                SplitAmount = splitAmount,
            });
        }

        public async Task<int> AddDrop(string item, int partyId, ulong discordServerId, IEnumerable<string> excludeList)
        {
            int itemId = await _dropItemDataAccess.GetRequiredDropItemId(item, discordServerId);
            IEnumerable<int> memberIds = await GetMemberIdListAfterExcludes(discordServerId, partyId, excludeList);

            return await InsertPartyDropRecord(itemId, partyId, memberIds);
        }

        public async Task<int> AddDrop(string item, string bossName, string bossDifficulty, ulong discordServerId, ulong discordUserId, IEnumerable<string> excludeList)
        {
            int partyId = await _partyDataAccess.GetUserPartyId(discordUserId, bossName, bossDifficulty, discordServerId);

            return await AddDrop(item, partyId, discordServerId, excludeList);
        }

        public async Task<int> AddDrop(string item, string bossAbbreviation, ulong discordServerId, ulong discordUserId, IEnumerable<string> excludeList)
        {
            int partyId = await _partyDataAccess.GetUserPartyId(discordUserId, bossAbbreviation, discordServerId);

            return await AddDrop(item, partyId, discordServerId, excludeList);
        }

        public async Task<int> AddDrop(string item, string partyName, ulong discordServerId, IEnumerable<string> excludeList)
        {
            int partyId = await _partyDataAccess.GetPartyId(partyName, discordServerId);

            return await AddDrop(item, partyId, discordServerId, excludeList);
        }

        private async Task<IEnumerable<int>> GetMemberIdListAfterExcludes(ulong discordServerId, int partyId, IEnumerable<string> excludeList)
        {
            IEnumerable<int> memberIds = await _partyDataAccess.GetMemberIds(partyId);

            List<int> excludedMemberIds = new();
            foreach (string memberName in excludeList)
            {
                int memberId = await _memberDataAccess.GetRequiredMemberId(memberName, discordServerId);

                if (!memberIds.Contains(memberId))
                {
                    throw new UserActionException(_embedUtilities.GetErrorEmbedBuilder($"'{memberName}' is not a member in this party!"));
                }

                excludedMemberIds.Add(memberId);
            }

            return memberIds.Except(excludedMemberIds);
        }
    }
}