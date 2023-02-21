# OnfootCops
Adds onfoot cops and some snipers at predefined locations

# Features:
* Standard Dispatch disabled while onfoot, and re-enabled when inside a vehicle (not compatible with script-based dispatch mods or it would work if it doesn't call ENABLE_DISPATCH_SERVICE native, but dispatch.meta edits work fine)
* Game will try to find a best suitable spawn location when attempting to spawn units
* Certain locations have vantage points for snipers to spawn from (only 1 sniper can exist at a time)
* Zone-Specific spawning (Broken atm, looking for native function that returns current zone)
* Diverse arsenal for SWAT units onfoot.
* DT_SwatHelicopter excluded, you will see units being dropped off from the air incase ground units can't reach you.
* You can configure some values of the mod by creating a .ini file with the same name as the DLL. To know which values you can use, take a look underneath me.

# Configuration:

```
[SETTINGS]
; Amount of onfoot units the game will spawn before no more
MAX_UNITS = 8
; Minimum distance police units can spawn from the player
MIN_POLICE_SPAWN_DISTANCE = 100f
; Minimum distance the player is required to be near a spawnpoint for snipers to spawn.
MIN_DISTANCE_FROM_SNIPER_SPAWNS = 500f
```
