using LutieBot.DataAccess.Models;
using SqlKata.Execution;

namespace LutieBot.DataAccess
{
    public class DropItemDataAccess
    {
        private readonly QueryFactory _db;

        public DropItemDataAccess(DataAccessMaster dataAccessMaster)
        {
            _db = dataAccessMaster.GetQueryFactory();
        }

        public async Task<IEnumerable<DropItemModel>> GetMatchingDropItems(string item)
        {
            var query = _db.Query();

            query.Select("DropItem.Id as Id")
                 .Select("DropItem.Name as ItemName")
                 .Select("DropItemAbbreviation.Abbreviation as ItemAbbreviation")
                 .From("DropItem")
                 .LeftJoin("DropItemAbbreviation", "DropItemAbbreviation.DropItemId", "DropItem.Id")
                 .WhereLike("DropItem.Name", item)
                 .OrWhereLike("DropItemAbbreviation.Abbreviation", item);

            return await query.GetAsync<DropItemModel>();
        }

        public async Task AddDropItem(int itemId, int quantity, int bossDifficultyId, int partyId, IEnumerable<MemberModel> members)
        {
            var partyDropRecordQuery = _db.Query("PartyDropRecord");

            for (int i = 0; i < quantity; i++)
            {
                int partyDropRecordId = await partyDropRecordQuery.InsertGetIdAsync<int>(new
                {
                    ItemId = itemId,
                    BossDifficultyId = bossDifficultyId,
                    PartyId = partyId,
                    Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                });

                var partyDropSplitQuery = _db.Query("PartyDropSplit");

                foreach (MemberModel member in members)
                {
                    await partyDropSplitQuery.InsertAsync(new
                    {
                        PartyDropRecordId = partyDropRecordId,
                        MemberId = member.Id
                    });
                }
            }
        }
    }
}
