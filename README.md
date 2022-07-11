# LutieBot

Discord bot for MapleStory party management. 

--- 

This is a massive work-in-progress. You **should not** be using it unless you know what you are doing. Many features are not implemented, database structure and feature plans can change at any time and compatibility is not considered at this stage. 

Anyways... 

### How to run

1. Get the [.NET 6.0 SDK](https://dotnet.microsoft.com/en-us/download).
2. Create a bot application under your Discord account in the [Discord Developers Portal](https://discord.com/developers/docs/intro). Invite the bot into your own server and copy the token.
3. Clone this repo. 
4. Paste the token into the `Token` field of `appsettings.json`. Also paste your Discord user ID into the `DeveloperIDs` list in `appsettings.json` (or set `IsDevMode` to `false` to accept commands without user restrictions). 
5. In a terminal, `cd` into the repo's folder and run `dotnet run`

### Database filling 

A clean SQLite database is available from this repo when you clone it (`Lutie.db`). However, as it is "clean" (as in contains zero data), you will need to fill in certain tables in the database yourself before using most commands in the bot. Commands that can be executed through Discord messages to insert these data into the database will be implemented in the future, but for now, it is what it is. 

You can use any database client that can interact with SQLite databases to do the job, such as [DBeaver](https://dbeaver.io/).

### Feature plans

A list of commands that are planned to be supported by the bot. This list is likely going to change in the future. 

| Command      	| Implementation Progress 	|
|--------------	|-------------------------	|
| ping         	| ✓                       	|
| add-party    	|                         	|
| remove-party 	|                         	|
| edit-party   	|                         	|
| party-info   	|WIP                       	|
| register     	|                         	|
| unregister   	|                         	|
| add-loot     	|WIP                       	|
| remove-loot  	|                         	|
| edit-loot    	|                         	|
| sell-loot    	|                         	|
| loot-info    	|                         	|
| mark-claimed 	|                         	|
|              	|                         	|

### Documentation 

(maybe coming soon.)

