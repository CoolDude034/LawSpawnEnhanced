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
* Spawned units will utilize tactics like chasing you down and carrying their wounded allies to safety.
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

; Time it takes before spawning another cop (was float before, now an int)
TIME_BETWEEN_SPAWNS = 80

; Difficulty level (range of 0-3, default being 1)
; Affects how much armor the peds will spawn with, and whether they can suffer critical hits or writhe
DIFFICULTY = 1

; Enable standard dispatch
ENABLE_STANDARD_SPAWNS = false

; Override the sniper model
OVERRIDE_SNIPER_MODEL = false

; Specify what models police snipers will use, if not provided will spawn the model by zone (e.g city/country = swat, army base = soldiers)
FORCED_SNIPER_MODEL = ""

; If true, mod will skip DT_PoliceHelicopter so police helis will always get dispatched, not just in vehicle.
ENABLE_POLICE_HELICOPTER = false

; If true, pacific standard bank area can spawn 2 police cars with a swat van to simulate a 'besiege'. Default: true
ENABLE_POLICE_BESIEGE = true

; Minimum distance from Pacific Standard Bank to trigger besiege event. Default: 100f
MIN_DISTANCE_FROM_BANK_FOR_POLICE_BESIEGE = 100f

; FORCE MODELS
; You can force peds spawned by the mod to use a specific model, good for addon peds
; If you want to change specific models per wanted level, set ENABLE_WANTED_LEVEL_SPECIFIC_OVERRIDES to true in your .ini file.
; Values you can change: COP_WL3, COP_WL4, COP_WL5

; City Cops
COP_MODEL_OVERRIDE = "s_m_y_swat_01"
; Countryside Cops
COP_COUNTRY_MODEL_OVERRIDE = "s_m_y_swat_01"
; Soldiers
ARMY_MODEL_OVERRIDE = "s_m_y_marine_03"
; Cayo Perico Guards (Defaults to s_m_y_swat)
CAYO_PERICO_GUARDS_OVERIRDE = "s_m_y_swat_01"
```
