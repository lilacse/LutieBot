namespace LutieBot.DatabaseMigrations.VersionData
{
    internal class DatabaseVersionDataProvider
    {
        // Version data in descending order. 
        private readonly List<VersionModel> _versionData;

        public DatabaseVersionDataProvider()
        {
            _versionData = new()
            {
                new VersionModel
                {
                    Version = 1,
                    Tables = new []
                    {
                        new TableModel
                        {
                            Name = "Boss",
                            Columns = new []
                            {
                                new ColumnModel
                                {
                                    Name = "Id",
                                    Type = ColumnType.Integer,
                                    IsPrimaryKey = true,
                                },
                                new ColumnModel
                                {
                                    Name = "Name",
                                    Type = ColumnType.Text,
                                },
                                new ColumnModel
                                {
                                    Name = "DiscordServerId",
                                    Type = ColumnType.Text,
                                },
                            },
                        },
                        new TableModel
                        {
                            Name = "BossDifficulty",
                            Columns = new []
                            {
                                new ColumnModel
                                {
                                    Name = "Id",
                                    Type = ColumnType.Integer,
                                    IsPrimaryKey = true,
                                },
                                new ColumnModel
                                {
                                    Name = "BossId",
                                    Type = ColumnType.Integer,
                                },
                                new ColumnModel
                                {
                                    Name = "Difficulty",
                                    Type = ColumnType.Text,
                                },
                            },
                        },
                        new TableModel
                        {
                            Name = "BossDifficultyAbbreviation",
                            Columns = new []
                            {
                                new ColumnModel
                                {
                                    Name = "Id",
                                    Type = ColumnType.Integer,
                                    IsPrimaryKey = true,
                                },
                                new ColumnModel
                                {
                                    Name = "BossDifficultyId",
                                    Type = ColumnType.Integer,
                                },
                                new ColumnModel
                                {
                                    Name = "Abbreviation",
                                    Type = ColumnType.Text,
                                },
                            },
                        },
                        new TableModel
                        {
                            Name = "DropItem",
                            Columns = new []
                            {
                                new ColumnModel
                                {
                                    Name = "Id",
                                    Type = ColumnType.Integer,
                                    IsPrimaryKey = true,
                                },
                                new ColumnModel
                                {
                                    Name = "Name",
                                    Type = ColumnType.Text,
                                },
                                new ColumnModel
                                {
                                    Name = "DiscordServerId",
                                    Type = ColumnType.Text,
                                },
                            },
                        },
                        new TableModel
                        {
                            Name = "DropDifficultyAbbreviation",
                            Columns = new []
                            {
                                new ColumnModel
                                {
                                    Name = "Id",
                                    Type = ColumnType.Integer,
                                    IsPrimaryKey = true,
                                },
                                new ColumnModel
                                {
                                    Name = "DropItemId",
                                    Type = ColumnType.Integer,
                                },
                                new ColumnModel
                                {
                                    Name = "Abbreviation",
                                    Type = ColumnType.Text,
                                },
                            },
                        },
                        new TableModel
                        {
                            Name = "Member",
                            Columns = new []
                            {
                                new ColumnModel
                                {
                                    Name = "Id",
                                    Type = ColumnType.Integer,
                                    IsPrimaryKey = true,
                                },
                                new ColumnModel
                                {
                                    Name = "Ign",
                                    Type = ColumnType.Text,
                                },
                                new ColumnModel
                                {
                                    Name = "Nickname",
                                    Type = ColumnType.Text,
                                    IsNotNull = false,
                                },
                                new ColumnModel
                                {
                                    Name = "DiscordId",
                                    Type = ColumnType.Text,
                                },
                                new ColumnModel
                                {
                                    Name = "DiscordServerId",
                                    Type = ColumnType.Text,
                                },
                            },
                        },
                        new TableModel
                        {
                            Name = "Party",
                            Columns = new []
                            {
                                new ColumnModel
                                {
                                    Name = "Id",
                                    Type = ColumnType.Integer,
                                    IsPrimaryKey = true,
                                },
                                new ColumnModel
                                {
                                    Name = "Name",
                                    Type = ColumnType.Text,
                                },
                                new ColumnModel
                                {
                                    Name = "BossDifficultyId",
                                    Type = ColumnType.Integer,
                                },
                                new ColumnModel
                                {
                                    Name = "DiscordServerId",
                                    Type = ColumnType.Text,
                                },
                            },
                        },
                        new TableModel
                        {
                            Name = "PartyDropRecord",
                            Columns = new []
                            {
                                new ColumnModel
                                {
                                    Name = "Id",
                                    Type = ColumnType.Integer,
                                    IsPrimaryKey = true,
                                },
                                new ColumnModel
                                {
                                    Name = "ItemId",
                                    Type = ColumnType.Integer,
                                },
                                new ColumnModel
                                {
                                    Name = "BossDifficultyId",
                                    Type = ColumnType.Integer,
                                },
                                new ColumnModel
                                {
                                    Name = "PartyId",
                                    Type = ColumnType.Integer,
                                },
                                new ColumnModel
                                {
                                    Name = "Timestamp",
                                    Type = ColumnType.Text,
                                },
                            },
                        },
                        new TableModel
                        {
                            Name = "PartyDropSplit",
                            Columns = new []
                            {
                                new ColumnModel
                                {
                                    Name = "Id",
                                    Type = ColumnType.Integer,
                                    IsPrimaryKey = true,
                                },
                                new ColumnModel
                                {
                                    Name = "PartyDropRecordId",
                                    Type = ColumnType.Integer,
                                },
                                new ColumnModel
                                {
                                    Name = "MemberId",
                                    Type = ColumnType.Integer,
                                },
                                new ColumnModel
                                {
                                    Name = "SplitAmount",
                                    Type = ColumnType.Integer,
                                    IsNotNull = false,
                                },
                                new ColumnModel
                                {
                                    Name = "Claimed",
                                    Type = ColumnType.Text,
                                    IsNotNull = false,
                                },
                                new ColumnModel
                                {
                                    Name = "ClaimedTimestamp",
                                    Type = ColumnType.Text,
                                    IsNotNull = false,
                                },
                            },
                        },
                        new TableModel
                        {
                            Name = "PartyMember",
                            Columns = new []
                            {
                                new ColumnModel
                                {
                                    Name = "Id",
                                    Type = ColumnType.Integer,
                                    IsPrimaryKey = true,
                                },
                                new ColumnModel
                                {
                                    Name = "PartyId",
                                    Type = ColumnType.Integer,
                                },
                                new ColumnModel
                                {
                                    Name = "MemberId",
                                    Type = ColumnType.Integer,
                                },
                            },
                        },
                    },
                    CreateScript = @"
                        CREATE TABLE _Version(
                            Version INTEGER NOT NULL);

                        INSERT INTO _Version VALUES (1);

                        CREATE TABLE Boss (
	                        Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	                        Name TEXT NOT NULL, 
                            DiscordServerId TEXT NOT NULL);

                        CREATE TABLE BossDifficulty (
	                        Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	                        BossId INTEGER NOT NULL,
	                        Difficulty TEXT NOT NULL,
	                        CONSTRAINT BossDifficulty_FK FOREIGN KEY (BossId) REFERENCES Boss(Id) ON DELETE RESTRICT ON UPDATE RESTRICT);

                        CREATE TABLE BossDifficultyAbbreviation (
	                        Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	                        BossDifficultyId INTEGER NOT NULL,
	                        Abbreviation TEXT NOT NULL,
	                        CONSTRAINT BossDifficultyAbbreviation_FK FOREIGN KEY (BossDifficultyId) REFERENCES BossDifficulty(Id) ON DELETE RESTRICT ON UPDATE RESTRICT);

                        CREATE TABLE DropItem (
	                        Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	                        Name TEXT NOT NULL, 
                            DiscordServerId TEXT NOT NULL);

                        CREATE TABLE DropItemAbbreviation (
	                        Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	                        DropItemId INTEGER NOT NULL,
	                        Abbreviation TEXT NOT NULL,
	                        CONSTRAINT DropItemAbbreviation_FK FOREIGN KEY (DropItemId) REFERENCES DropItem(Id) ON DELETE RESTRICT ON UPDATE RESTRICT);

                        CREATE TABLE ""Member"" (
	                        Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	                        Ign TEXT NOT NULL,
	                        Nickname TEXT,
	                        DiscordId TEXT NOT NULL,
	                        DiscordServerId TEXT NOT NULL);

                        CREATE TABLE Party (
	                        Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	                        Name TEXT NOT NULL,
	                        BossDifficultyId INTEGER NOT NULL,
	                        DiscordServerId TEXT NOT NULL,
	                        CONSTRAINT Party_FK FOREIGN KEY (BossDifficultyId) REFERENCES BossDifficulty(Id) ON DELETE RESTRICT ON UPDATE RESTRICT);

                        CREATE TABLE PartyMember (
	                        Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	                        PartyId INTEGER NOT NULL,
	                        MemberId INTEGER NOT NULL,
	                        CONSTRAINT PartyMember_FK FOREIGN KEY (PartyId) REFERENCES Party(Id) ON DELETE RESTRICT ON UPDATE RESTRICT,
	                        CONSTRAINT PartyMember_FK_1 FOREIGN KEY (MemberId) REFERENCES ""Member""(Id) ON DELETE RESTRICT ON UPDATE RESTRICT);

                        CREATE TABLE PartyDropRecord (
	                        Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	                        ItemId INTEGER NOT NULL,
	                        BossDifficultyId INTEGER NOT NULL,
	                        PartyId INTEGER NOT NULL,
	                        ""Timestamp"" TEXT NOT NULL,
                            CONSTRAINT PartyDropRecord_FK FOREIGN KEY (ItemId) REFERENCES DropItem(Id) ON DELETE RESTRICT ON UPDATE RESTRICT,
                            CONSTRAINT PartyDropRecord_FK_1 FOREIGN KEY (BossDifficultyId) REFERENCES BossDifficulty(Id) ON DELETE RESTRICT ON UPDATE RESTRICT,
                            CONSTRAINT PartyDropRecord_FK_2 FOREIGN KEY (PartyId) REFERENCES Party(Id) ON DELETE RESTRICT ON UPDATE RESTRICT);

                        CREATE TABLE PartyDropSplit (
	                        Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	                        PartyDropRecordId INTEGER NOT NULL,
	                        MemberId INTEGER NOT NULL,
	                        SplitAmount INTEGER,
	                        Claimed TEXT,
	                        ClaimedTimestamp TEXT,
	                        CONSTRAINT PartyDropSplit_FK FOREIGN KEY (PartyDropRecordId) REFERENCES PartyDropRecord(Id) ON DELETE RESTRICT ON UPDATE RESTRICT,
	                        CONSTRAINT PartyDropSplit_FK_1 FOREIGN KEY (MemberId) REFERENCES ""Member""(Id) ON DELETE RESTRICT ON UPDATE RESTRICT);",
                },
            };
        }

        public VersionModel GetLatestVersion()
        {
            return _versionData[0];
        }

        public VersionModel? GetVersion(int version)
        {
            return _versionData.Where(v => v.Version == version).FirstOrDefault();
        }

        public IEnumerable<string> GetUpdateScripts(int version)
        {
            return _versionData.TakeWhile(v => v.Version != version).Select(v => v.UpdateScript).Reverse();
        }
    }
}
