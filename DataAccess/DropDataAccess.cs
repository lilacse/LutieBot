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

        public DropDataAccess(DataAccessMaster dataAccessMaster, EmbedUtilities embedUtilities, DropItemDataAccess dropItemDataAccess, PartyDataAccess partyDataAccess)
        {
            _db = dataAccessMaster.GetQueryFactory();
            _embedUtilities = embedUtilities;
            _dropItemDataAccess = dropItemDataAccess;
            _partyDataAccess = partyDataAccess;
        }

        public async Task<int> InsertPartyDropRecord(int itemId, int partyId)
        {
            var partyDropRecordQuery = _db.Query("PartyDropRecord");

            IEnumerable<int> memberIds = await _partyDataAccess.GetMemberIds(partyId);

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

        public async Task<int> AddDrop(string item, string bossName, string bossDifficulty, ulong discordServerId, ulong discordUserId)
        {
            int itemId = await _dropItemDataAccess.GetRequiredDropItemId(item, discordServerId);
            int partyId = await _partyDataAccess.GetUserPartyId(discordUserId, bossName, bossDifficulty, discordServerId);

            return await InsertPartyDropRecord(itemId, partyId);
        }

        public async Task<int> AddDrop(string item, string bossAbbreviation, ulong discordServerId, ulong discordUserId)
        {
            int itemId = await _dropItemDataAccess.GetRequiredDropItemId(item, discordServerId);
            int partyId = await _partyDataAccess.GetUserPartyId(discordUserId, bossAbbreviation, discordServerId);

            return await InsertPartyDropRecord(itemId, partyId);
        }

        public async Task<int> AddDrop(string item, string partyName, ulong discordServerId)
        {
            int itemId = await _dropItemDataAccess.GetRequiredDropItemId(item, discordServerId);
            int partyId = await _partyDataAccess.GetPartyId(partyName, discordServerId);

            return await InsertPartyDropRecord(itemId, partyId);
        }
    }
}