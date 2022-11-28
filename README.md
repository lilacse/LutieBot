# LutieBot

Discord bot for MapleStory party management. 

--- 

This is a massive work-in-progress. You **should not** be using it unless you know what you are doing. Many features are not implemented, database structure and feature plans can change at any time and compatibility is not considered at this stage. 

Anyways... 

### How to run

1. Get the [.NET 6.0 SDK](https://dotnet.microsoft.com/en-us/download).
2. Create a bot application under your Discord account in the [Discord Developers Portal](https://discord.com/developers/docs/intro). Invite the bot into your own server and copy the token.
3. Clone this repo. 
4. Paste the token into the `Token` field of `appsettings.json`. Also paste your Discord server ID into the `DiscordServerIDs` list in `appsettings.json` (or set `IsDevMode` to `false` to register commands globally). 
5. In a terminal, `cd` into the repo's folder and run `dotnet run`

### Feature plans

A list of commands that are planned to be supported by the bot. This list is likely going to change in the future. 

(some progress got reseted because they are going to be rewritten in slash commands.)

| Command       	| Implementation Progress 	|
|------------------ |-------------------------	|
| ping           	| ✓                       	|
| new-party     	| ✓                        	|
| delete-party  	|                         	|
| edit-party    	|                         	|
| get-party     	| ✓                        	|
| register       	| ✓                        	|
| register-member   | ✓                         |
| unregister   	    |                         	|
| unregister-member |                           |
| get-profile       |                           |
| edit-profile      |                           |
| new-boss          | ✓                         |
| delete-boss       |                           |
| edit-boss         |                           |
| get-boss          |                           |
| new-drop-item     | ✓                         |
| delete-drop-item  |                           |
| edit-drop-item    |                           |
| get-drop-item     |                           |
| add-drop      	| ✓                      	|
| remove-drop   	|                         	|
| edit-drop     	|                         	|
| get-drops     	|                         	|
| sell-drop     	|                         	|
| mark-claimed     	|                         	|
|               	|                         	|

### Documentation 

(maybe coming soon.)

