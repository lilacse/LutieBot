using LutieBot.Exceptions;
using LutieBot.Utilities;
using SqlKata.Execution;

namespace LutieBot.DataAccess
{
    public class DropItemDataAccess
    {
        private readonly QueryFactory _db;
        private readonly EmbedUtilities _embedUtilities;

        public DropItemDataAccess(DataAccessMaster dataAccessMaster, EmbedUtilities embedUtilities)
        {
            _db = dataAccessMaster.GetQueryFactory();
            _embedUtilities = embedUtilities;
        }

        public async Task<int> GetDropItemId(string itemName, ulong discordServerId)
        {
            return await _db.Query("DropItem")
                            .Select("Id")
                            .WhereLike("Name", itemName)
                            .Where("DiscordServerId", discordServerId)
                            .FirstOrDefaultAsync<int>();
        }

        public async Task AddDropItem(string itemName, IEnumerable<string> abbreviations, ulong discordServerId)
        {
            if (await GetDropItemId(itemName, discordServerId) != 0)
            {
                throw new UserActionException(_embedUtilities.GetErrorEmbedBuilder($"The drop item \"{itemName}\" already exists!"));
            }

            var dropItemQuery = _db.Query("DropItem");

            int dropItemId = await dropItemQuery.InsertGetIdAsync<int>(new
            {
                Name = itemName, 
                DiscordServerId = discordServerId
            });

            var dropItemAbbreviationQuery = _db.Query("DropItemAbbreviation");
            foreach (string abbr in abbreviations)
            {
                await dropItemAbbreviationQuery.InsertAsync(new
                {
                    DropItemId = dropItemId, 
                    Abbreviation = abbr
                });
            }
        }
    }
}

