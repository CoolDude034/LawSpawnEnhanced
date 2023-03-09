# LawSpawnEnhanced
Extends the spawning of police units instead always coming from vehicles only. Disables certain dispatches to make that happen, so it's incompatible with other
script-based dispatch mods (or even missions but im sure missions utilize FakeWantedLevel), but can be used with things that only modify ingame files.
Cops spawned trough scenarios or popcycle aren't touched.

# Features:
* Standard Dispatch disabled while onfoot, and re-enabled when inside a vehicle (not compatible with script-based dispatch mods or it would work if it doesn't call ENABLE_DISPATCH_SERVICE native, but dispatch.meta edits work fine)
* Game will try to find a best suitable spawn location when attempting to spawn units, e.g sidewalks
* Certain locations have vantage points for snipers to spawn from (only 1 sniper can exist at a time)
* Diverse arsenal for SWAT units onfoot.
* DT_SwatHelicopter excluded, you will see units being dropped off from the air incase ground units can't reach you.
* You can configure some values of the mod by creating a .ini file with the same name as the DLL. To know which values you can use, take a look underneath me.

# Configuration:

```
[SETTINGS]

; Amount of onfoot units the game will spawn before no more
MAX_UNITS = 8

; Minimum WantedLevel required for onfoot cops to start spawning.
MAX_WANTED_LEVEL = 3

; Minimum distance police units can spawn from the player
MIN_POLICE_SPAWN_DISTANCE = 100f

; Minimum distance the player is required to be near a spawnpoint for snipers to spawn.
MIN_DISTANCE_FROM_SNIPER_SPAWNS = 500f

; Min distance from player the police will despawn.
MIN_POLICE_DESPAWN_RANGE = 600f
```
