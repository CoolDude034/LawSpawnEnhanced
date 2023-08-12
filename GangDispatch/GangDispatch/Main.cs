using System;
using System.Collections.Generic;
using GTA;
using GTA.Math;
using GTA.Native;

namespace GangDispatch
{
    public class Main : Script
    {

        List<Ped> groups = new List<Ped>();

        Random random = new Random();
        Ped sniper;
        bool isSniperSpawned = false;
        bool isPoliceBesiegeSpawned = false;
        bool canSpawnPeds = false;
        Vector3 policeBesiegeLocation = new Vector3(204.728027f, 201.1926f, 104.5698f);

        // Models
        Model[] assault_weapons = { WeaponHash.SMG, WeaponHash.CarbineRifle, WeaponHash.PumpShotgun, WeaponHash.AssaultShotgun };

        // Private variables
        int MAX_UNITS;
        int MAX_WANTED_LEVEL;
        int TIME_BETWEEN_SPAWNS; // now a intvalue
        int DIFFICULTY;
        int INITIAL_SPAWN_DELAY;
        float MIN_POLICE_SPAWN_DISTANCE;
        float MIN_DISTANCE_FROM_SNIPER_SPAWNS;
        float MIN_POLICE_DESPAWN_RANGE;
        float MIN_DISTANCE_FROM_BANK_FOR_POLICE_BESIEGE;
        bool ENABLE_STANDARD_SPAWNS;
        bool ENABLE_POLICE_HELICOPTER;
        bool ENABLE_POLICE_BESIEGE;
        bool ENABLE_OLD_WEAPON_SYSTEM;
        string COP_MODEL_OVERRIDE;
        string COP_COUNTRY_MODEL_OVERRIDE;
        string CAYO_PERICO_GUARDS_OVERIRDE;
        string ARMY_MODEL_OVERRIDE;

        string LOADOUT_SET;

        // Wanted Level Specific Overrides
        bool ENABLE_WANTED_LEVEL_SPECIFIC_OVERRIDES;
        string COP_WL3;
        string COP_WL4;
        string COP_WL5;

        bool OVERRIDE_SNIPER_MODEL;
        string FORCED_SNIPER_MODEL;

        Vector3[] SniperSpawns =
        {
            new Vector3(200.027817f, 248.806641f, 140.4281f),
            new Vector3(213.058029f, 166.939468f, 136.523026f),
            new Vector3(167.033127f, 153.216919f, 120.9819f),
            new Vector3(221.321091f, 118.152344f, 112.676796f),
            new Vector3(-35.6877823f, -3.81287861f, 82.65021f),
            new Vector3(-26.5661736f, 5.56382561f, 82.65021f),
            new Vector3(115.254715f, -1039.75647f, 56.8019829f),
            new Vector3(118.102577f, -1031.68579f, 56.8019829f),
            new Vector3(320.6497f, -1026.81958f, 66.10583f),
            new Vector3(312.858276f, -1026.81958f, 66.10583f),
            new Vector3(438.792725f, -928.5377f, 44.70594f),
            new Vector3(426.929321f, -928.5377f, 44.70594f),
            new Vector3(392.872131f, -890.8673f, 38.1645775f),
            new Vector3(416.605133f, -883.9329f, 43.5661354f),
            new Vector3(425.41684f, -872.658569f, 43.5643f),
            new Vector3(346.15097f, -833.099854f, 66.10499f),
            new Vector3(320.432037f, -828.9054f, 66.10499f),
            // Blitz Play Location :D
            new Vector3(925.3938f, -2376.73364f, 40.17347f),
            new Vector3(928.1308f, -2341.068f, 38.8358459f),
            new Vector3(923.5157f, -2399.00366f, 40.1732559f),
            new Vector3(839.927063f, -2305.346f, 50.8185f),
            new Vector3(804.83f, -2311.378f, 54.57426f),
            // Downtown LS
            new Vector3(-270.925323f, -585.946838f, 51.01512f),
            new Vector3(-270.925323f, -599.1961f, 51.01512f),
            new Vector3(-324.855164f, -619.5216f, 58.4729729f),
            new Vector3(-575.951965f, -1049.85181f, 32.37606f),
            new Vector3(-584.3051f, -1029.52808f, 32.3760948f),
            new Vector3(-600f, -705.611633f, 47.22113f),
            new Vector3(-575.318848f, -705.611633f, 47.22113f),
            new Vector3(-731.774536f, -721.693848f, 43.9671555f),
            new Vector3(10.21494f, 135.91217f, 103.1198f),
            new Vector3(133.841858f, 81.19723f, 95.14065f),
            new Vector3(128.89241f, 66.70337f, 95.14065f),
            // ResDistr Rooftop Snipers
            new Vector3(-999.821167f, -1207.19641f, 14.3100691f),
            new Vector3(-1089.90051f, -1229.69678f, 13.4221287f),
            // Port Area near LSIA
            new Vector3(-440.312134f, -2816.76172f, 16.4541035f),
            new Vector3(-430.2499f, -2825.95044f, 16.4541626f),
            new Vector3(-330.218872f, -2779.36426f, 12.6000147f),
            // Mission Row
            new Vector3(352.3809f, -967.0717f, 34.6470642f),
            new Vector3(360.449371f, -967.0417f, 34.6899643f),
            // idk where this is
            new Vector3(-51.0848846f, 188.77771f, 140.179108f),
            new Vector3(177.156265f, 1233.22241f, 233.833328f),
            // Fort Zancudo
            new Vector3(-1720.31677f, 3152.416f, 50.93837f),
            // Paleto Bay
            new Vector3(-176.23349f, 6337.69434f, 35.10514f),
            new Vector3(-67.0642548f, 6441.175f, 39.37759f),
            new Vector3(-93.05518f, 6498.224f, 40.3665581f),
            new Vector3(-55.56639f, 6503.51074f, 38.4160538f),
            new Vector3(-66.81067f, 6268.21045f, 46.7211342f),
            new Vector3(-439.295624f, 6015.17871f, 35.6452179f), // camper
            // Vespucci PD
            new Vector3(-1092.56323f, -811.278137f, 30.26501f),
            new Vector3(-1074.167f, -846.214233f, 14.6220026f),
            // These we're from my old sniper mod, thought it would be cool to include these in this mod.
            // Don't ask where these are located, i do not know. I guess explore LOL
            new Vector3(-1041.913f, -2533.781f, 30.457f),
            new Vector3(-1164.703f, -2446.158f, 36.041f),
            new Vector3(-1080.765f, -2686.747f, 34.31932f),
            new Vector3(-1110.262f, -2701.137f, 20.38633f),
            new Vector3(-240.6813f, -197.2891f, 77.33955f),
            new Vector3(-211.2993f, -260.3657f, 77.33955f),
            new Vector3(-587.3903f, -724.0382f, 128.2443f),
            new Vector3(-601.7745f, -934.7025f, 35.90825f),
            new Vector3(386.6986f, -1015.355f, 57.88143f),
            new Vector3(434.2407f, -928.4073f, 44.76479f),
            new Vector3(-826.342f, -616.4815f, 95.19798f),
            new Vector3(-773.8983f, -633.6242f, 95.19798f),
            new Vector3(-354.7279f, -1044.218f, 72.88655f),
            new Vector3(59.95131f, -1006.621f, 78.83146f),
            new Vector3(23.24099f, -993.4604f, 82.38417f),
            new Vector3(251.5045f, -1026.515f, 60.59642f),
            new Vector3(279.6348f, -820.5026f, 71.63631f),
            new Vector3(270.1202f, -832.9045f, 71.63603f),
            new Vector3(363.6951f, -707.6724f, 84.61191f),
            new Vector3(371.2715f, -1584.88f, 35.94881f),
        };

        void AddHelmet(Ped ped)
        {
            Function.Call(Hash.SET_PED_PROP_INDEX, ped, 0, 0, 0, true);
        }

        void SetComponent(Ped ped)
        {
            if (DIFFICULTY == 2 && Game.Player.WantedLevel == 5)
            {
                Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped, 10, 0, 1, 0);
            }
            else if (DIFFICULTY == 3 && Game.Player.WantedLevel >= 4)
            {
                Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped, 10, 0, 1, 0);
            }
        }

        // There was no reason for this to return anything, so just return void
        void SpawnUnit(Vector3 pos)
        {
            var model = GetModelByZone();
            if (!model.IsLoaded)
            {
                model.Request(5);
            }

            if (model.IsInCdImage && model.IsValid)
            {
                var ped = World.CreatePed(model, pos);
                ped.Task.FightAgainst(Game.Player.Character);

                if (ENABLE_OLD_WEAPON_SYSTEM)
                {
                    var weapon = assault_weapons[random.Next(0, assault_weapons.Length)];
                    ped.Weapons.Give(weapon, 9999, true, true);
                }
                else
                {
                    var hash = Function.Call<int>(Hash.GET_HASH_KEY, LOADOUT_SET);
                    Function.Call(Hash.GIVE_LOADOUT_TO_PED, ped, hash);
                }

                if (DIFFICULTY == 1)
                {
                    ped.Armor += 5;
                }
                else if (DIFFICULTY == 2)
                {
                    ped.Armor += 15;
                }
                else if (DIFFICULTY == 3)
                {
                    ped.Armor += 20;
                }

                AddHelmet(ped);
                SetComponent(ped);
                Function.Call(Hash.SET_PED_COMBAT_MOVEMENT, ped, 2); // CM_WillAdvance
                Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 46, true); // BF_AlwaysFight
                Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 21, true); // chase target onfoot
                Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 22, true); // drag injured *comrades* to safety
                Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 28, true);
                Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 60, true); // allow throw smoke grenades
                Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 41, true);
                Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 42, true);

                groups.Add(ped);
            }
        }

        // meet the sniper
        void SpawnSniper(Vector3 pos)
        {
            if (isSniperSpawned) return;

            var model = OVERRIDE_SNIPER_MODEL ? new Model(FORCED_SNIPER_MODEL) : GetModelByZone();
            if (!model.IsLoaded)
            {
                model.Request(5);
            }

            if (model.IsInCdImage && model.IsValid)
            {
                sniper = World.CreatePed(model, pos);
                sniper.AlwaysKeepTask = true;
                sniper.CanWrithe = false;
                sniper.Task.FightAgainst(Game.Player.Character);

                sniper.Weapons.Give(WeaponHash.SniperRifle, 9999, true, true);

                AddHelmet(sniper);
                SetComponent(sniper);
                Function.Call(Hash.SET_PED_COMBAT_MOVEMENT, sniper, 0);
                Function.Call(Hash.SET_PED_COMBAT_RANGE, sniper, 3);
                Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, sniper, 46, true); // BF_AlwaysFight
                Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, sniper, 21, false); // DONT chase target onfoot
                Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, sniper, 22, false); // DONT drag injured *comrades* to safety
                Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, sniper, 27, true);

                isSniperSpawned = true;
            }
        }

        public Main()
        {
            ScriptSettings.Load("scripts/" + this.Filename + ".ini");
            MAX_UNITS = Settings.GetValue<int>("SETTINGS", "MAX_UNITS", 8);
            MAX_WANTED_LEVEL = Settings.GetValue<int>("SETTINGS", "MAX_WANTED_LEVEL", 3);
            MIN_POLICE_SPAWN_DISTANCE = Settings.GetValue<float>("SETTINGS", "MIN_POLICE_SPAWN_DISTANCE", 100f);
            MIN_DISTANCE_FROM_SNIPER_SPAWNS = Settings.GetValue<float>("SETTINGS", "MIN_DISTANCE_FROM_SNIPER_SPAWNS", 500f);
            MIN_POLICE_DESPAWN_RANGE = Settings.GetValue<float>("SETTINGS", "MIN_POLICE_DESPAWN_RANGE", 600f);
            TIME_BETWEEN_SPAWNS = Settings.GetValue<int>("SETTINGS", "TIME_BETWEEN_SPAWNS", 80);
            ENABLE_STANDARD_SPAWNS = Settings.GetValue<bool>("SETTINGS", "ENABLE_STANDARD_SPAWNS", false);
            ENABLE_POLICE_HELICOPTER = Settings.GetValue<bool>("SETTINGS", "ENABLE_POLICE_HELICOPTER", false);
            ENABLE_POLICE_BESIEGE = Settings.GetValue<bool>("SETTINGS", "ENABLE_POLICE_BESIEGE", true);
            ENABLE_OLD_WEAPON_SYSTEM = Settings.GetValue<bool>("SETTINGS", "ENABLE_OLD_WEAPON_SYSTEM", false);
            MIN_DISTANCE_FROM_BANK_FOR_POLICE_BESIEGE = Settings.GetValue<float>("SETTINGS", "MIN_DISTANCE_FROM_BANK_FOR_POLICE_BESIEGE", 100f);
            INITIAL_SPAWN_DELAY = Settings.GetValue<int>("SETTINGS", "INITIAL_SPAWN_DELAY", 120);
            LOADOUT_SET = Settings.GetValue<string>("SETTINGS", "LOADOUT_SET", "LOADOUT_SWAT");
            // Updated code to look for modelnames instead of pedhashes, note this means that you need to update your config otherwise it will fallback to swat
            // civmale/civfemale pedtypes can cause in-fighting between them
            COP_MODEL_OVERRIDE = Settings.GetValue<string>("SETTINGS", "COP_MODEL_OVERRIDE", "s_m_y_swat_01");
            ARMY_MODEL_OVERRIDE = Settings.GetValue<string>("SETTINGS", "ARMY_MODEL_OVERRIDE", "s_m_y_marine_03");
            COP_COUNTRY_MODEL_OVERRIDE = Settings.GetValue<string>("SETTINGS", "COP_COUNTRY_MODEL_OVERRIDE", COP_MODEL_OVERRIDE);
            CAYO_PERICO_GUARDS_OVERIRDE = Settings.GetValue<string>("SETTINGS", "CAYO_PERICO_GUARDS_OVERIRDE", COP_MODEL_OVERRIDE);

            ENABLE_WANTED_LEVEL_SPECIFIC_OVERRIDES = Settings.GetValue<bool>("SETTINGS", "ENABLE_WANTED_LEVEL_SPECIFIC_OVERRIDES", false);
            COP_WL3 = Settings.GetValue<string>("SETTINGS", "COP_WL3", COP_MODEL_OVERRIDE);
            COP_WL4 = Settings.GetValue<string>("SETTINGS", "COP_WL4", COP_MODEL_OVERRIDE);
            COP_WL5 = Settings.GetValue<string>("SETTINGS", "COP_WL5", COP_MODEL_OVERRIDE);

            OVERRIDE_SNIPER_MODEL = Settings.GetValue<bool>("SETTINGS", "OVERRIDE_SNIPER_MODEL", false);
            FORCED_SNIPER_MODEL = Settings.GetValue<string>("SETTINGS", "FORCED_SNIPER_MODEL", COP_MODEL_OVERRIDE);

            // Difficulty Values are 0-3
            DIFFICULTY = Settings.GetValue<int>("SETTINGS", "DIFFICULTY", 1);
            if (DIFFICULTY > 3 || DIFFICULTY < 0)
            {
                DIFFICULTY = 1;
            }

            Tick += OnTick;
            Aborted += ScriptCleanup;
        }

        bool canSpawn()
        {
            return (groups.Count < MAX_UNITS);
        }

        bool isInSniperLocation()
        {
            for (int i = 1; i < SniperSpawns.Length; i++)
            {
                var pos = SniperSpawns[i];
                if (Game.Player.Character.Position.DistanceTo(pos) < MIN_DISTANCE_FROM_SNIPER_SPAWNS)
                {
                    return true;
                }
            }

            return false;
        }

        bool isForcedToSidewalks()
        {
            if (GetZoneType() == "CAYO_PERICO")
            {
                return false;
            }

            return true;
        }

        Vector3 FindNearestSniperSpawn()
        {
            var random_spawn = SniperSpawns[random.Next(0, SniperSpawns.Length)];

            if (Game.Player.Character.Position.DistanceTo(random_spawn) < MIN_DISTANCE_FROM_SNIPER_SPAWNS)
            {
                return random_spawn;
            }

            return Vector3.Zero;
        }

        Vector3 FindAvailableSpawnPoint()
        {
            Vector3[] randomPos = { Game.Player.Character.ForwardVector * MIN_POLICE_SPAWN_DISTANCE, Game.Player.Character.ForwardVector * -MIN_POLICE_SPAWN_DISTANCE, Game.Player.Character.RightVector * MIN_POLICE_SPAWN_DISTANCE, Game.Player.Character.RightVector * -MIN_POLICE_SPAWN_DISTANCE };
            Vector3 newPos = randomPos[random.Next(0, randomPos.Length)];
            Vector3 pos = World.GetSafeCoordForPed(Game.Player.Character.Position + newPos, sidewalk: isForcedToSidewalks());

            return pos;
        }

        Model GetModelByZone()
        {
            if (GetZoneType() == "ARMY")
            {
                return new Model(ARMY_MODEL_OVERRIDE);
            }
            else if (GetZoneType() == "DESERT")
            {
                return new Model(COP_COUNTRY_MODEL_OVERRIDE);
            }
            else if (GetZoneType() == "CAYO_PERICO")
            {
                return new Model(CAYO_PERICO_GUARDS_OVERIRDE);
            }

            if (ENABLE_WANTED_LEVEL_SPECIFIC_OVERRIDES)
            {
                switch(Game.Player.WantedLevel)
                {
                    case 3:
                        return new Model(COP_WL3);
                    case 4:
                        return new Model(COP_WL4);
                    case 5:
                        return new Model(COP_WL5);
                }
            }

            return new Model(COP_MODEL_OVERRIDE);
        }

        string GetZoneType()
        {
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ArmyB") || Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zancudo"))
            {
                return "ARMY";
            }
            else if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Desrt") || Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Alamo") || Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Lago") || Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Slab") || Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Paleto") || Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Sandy"))
            {
                return "DESERT";
            }
            else if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "IsHeistZone"))
            {
                return "CAYO_PERICO";
            }

            return "DEFAULT";
        }

        void UpdateState()
        {
            if (Game.Player.WantedLevel >= MAX_WANTED_LEVEL)
            {
                if (!canSpawnPeds && Game.GameTime % INITIAL_SPAWN_DELAY == 0)
                {
                    canSpawnPeds = true;
                }

                switch (Game.Player.Character.IsOnFoot)
                {
                    case true:
                        ToggleDispatchServices(false);
                        break;
                    case false:
                        ToggleDispatchServices(true);
                        break;
                }
            }
            else
            {
                // Reset canSpawnPeds value back to false
                if (Game.Player.WantedLevel <= 0)
                {
                    canSpawnPeds = false;
                }
                ToggleDispatchServices(true);
            }
        }

        void SpawnGroup()
        {
            var pos = FindAvailableSpawnPoint();
            if (pos != Vector3.Zero)
            {
                SpawnUnit(pos);
            }
        }

        void UpdateGroups()
        {
            if (Game.Player.WantedLevel >= MAX_WANTED_LEVEL && canSpawn() && canSpawnPeds)
            {
                bool IS_SEEN_BY_COPS = Function.Call<bool>(Hash.IS_WANTED_AND_HAS_BEEN_SEEN_BY_COPS, Game.Player);
                if (Game.Player.Character.IsOnFoot && IS_SEEN_BY_COPS)
                {
                    if (Game.GameTime % TIME_BETWEEN_SPAWNS == 0)
                    {
                        SpawnGroup();
                    }
                }
            }
        }

        void ClearAllAssaultingMembers(bool isForced = false)
        {
            if (groups.Count > 0)
            {
                for (int i = groups.Count - 1; i > -1; i--)
                {
                    var ped = groups[i];
                    if (ped != null && ped.Exists())
                    {
                        if (isForced)
                        {
                            ped.AlwaysKeepTask = false;
                            ped.MarkAsNoLongerNeeded();
                            groups.RemoveAt(i);
                        }
                        else
                        {
                            if (ped.IsDead || Game.Player.WantedLevel <= 0 || Game.Player.IsDead || World.GetDistance(ped.Position, Game.Player.Character.Position) > MIN_POLICE_DESPAWN_RANGE)
                            {
                                ped.AlwaysKeepTask = false;
                                ped.MarkAsNoLongerNeeded();
                                groups.RemoveAt(i);
                            }
                        }
                    }
                }

            }
        }

        void RespawnSnipers()
        {
            if (Game.Player.WantedLevel >= 4 && isInSniperLocation())
            {
                var pos = FindNearestSniperSpawn();

                if (pos != Vector3.Zero && Game.GameTime % TIME_BETWEEN_SPAWNS == 0)
                {
                    SpawnSniper(pos);
                }
            }
        }

        void UpdateExistingMembers()
        {

            if (sniper != null && sniper.Exists())
            {
                if (World.GetDistance(sniper.Position, Game.Player.Character.Position) > MIN_DISTANCE_FROM_SNIPER_SPAWNS || Game.Player.WantedLevel <= 0 || sniper.IsDead)
                {
                    sniper.MarkAsNoLongerNeeded();
                    sniper = null;
                    isSniperSpawned = false;
                }
            }
            else
            {
                RespawnSnipers();
            }

            ClearAllAssaultingMembers();
        }

        void SpawnPoliceBesiege()
        {
            if (!ENABLE_POLICE_BESIEGE) return;
            if (Game.Player.WantedLevel >= MAX_WANTED_LEVEL && Game.Player.Character.Position.DistanceTo(policeBesiegeLocation) < MIN_DISTANCE_FROM_BANK_FOR_POLICE_BESIEGE && !isPoliceBesiegeSpawned)
            {
                var policeCar1 = World.CreateVehicle(VehicleHash.Police, new Vector3(183.307083f, 193.525482f, 104.558456f));
                var policeCar2 = World.CreateVehicle(VehicleHash.Police, new Vector3(184.620712f, 208.190582f, 104.73996f), heading: -2.87979341f);
                var swatVan = World.CreateVehicle(VehicleHash.Riot, new Vector3(232.5081f, 278.516174f, 104.590385f), heading: -2.70526052f);
                var exists = policeCar1 != null && policeCar1.Exists() && policeCar2 != null && policeCar2.Exists() && swatVan != null && swatVan.Exists();
                if (exists)
                {
                    if (policeCar1.HasSiren)
                    {
                        policeCar1.IsSirenActive = true;
                        policeCar1.IsSirenSilent = true;
                    }
                    if (policeCar2.HasSiren)
                    {
                        policeCar2.IsSirenActive = true;
                        policeCar2.IsSirenSilent = true;
                    }
                    if (swatVan.HasSiren)
                    {
                        swatVan.IsSirenActive = true;
                        swatVan.IsSirenSilent = true;
                    }
                    policeCar1.PlaceOnGround();
                    policeCar2.PlaceOnGround();
                    swatVan.PlaceOnGround();
                    policeCar1.MarkAsNoLongerNeeded();
                    policeCar2.MarkAsNoLongerNeeded();
                    swatVan.MarkAsNoLongerNeeded();
                }

                // Spawn Cops
                var cop1 = World.CreatePed(PedHash.Cop01SMY, new Vector3(179.075043f, 206.704636f, 104.994026f), heading: -1.832596f);
                if (cop1 != null && cop1.Exists())
                {
                    cop1.Weapons.Give(WeaponHash.Pistol, 9999, true, true);
                    cop1.Task.FightAgainst(Game.Player.Character);
                    Function.Call(Hash.SET_PED_COMBAT_MOVEMENT, cop1, 0);
                    Function.Call(Hash.SET_PED_COMBAT_RANGE, cop1, 3);
                    Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, cop1, 46, true);
                    cop1.MarkAsNoLongerNeeded();
                }

                var sniper1 = World.CreatePed(PedHash.Swat01SMY, new Vector3(176.758759f, 198.06012f, 104.948158f), heading: -1.832596f);
                if (sniper1 != null && sniper1.Exists())
                {
                    sniper1.AlwaysKeepTask = true;
                    sniper1.Weapons.Give(WeaponHash.SniperRifle, 9999, true, true);
                    sniper1.Task.FightAgainst(Game.Player.Character);
                    Function.Call(Hash.SET_PED_COMBAT_MOVEMENT, sniper1, 0);
                    Function.Call(Hash.SET_PED_COMBAT_RANGE, sniper1, 3);
                    Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, sniper1, 46, true);
                    sniper1.MarkAsNoLongerNeeded();
                }

                isPoliceBesiegeSpawned = true;
            }
            else if (isPoliceBesiegeSpawned)
            {
                if (Game.Player.WantedLevel <= 0 || Game.Player.Character.Position.DistanceTo(policeBesiegeLocation) > MIN_DISTANCE_FROM_BANK_FOR_POLICE_BESIEGE)
                {
                    isPoliceBesiegeSpawned = false;
                }
            }
        }

        void OnTick(object sender, EventArgs e)
        {
            UpdateState();
            UpdateGroups();
            UpdateExistingMembers();
            SpawnPoliceBesiege();
        }

        void ScriptCleanup(object sender, EventArgs e)
        {
            Tick -= OnTick;

            if (sniper != null && sniper.Exists())
            {
                sniper.MarkAsNoLongerNeeded();
                sniper = null;
                isSniperSpawned = false;
            }

            ClearAllAssaultingMembers(isForced: true);
            ToggleDispatchServices(true);
        }

        void ToggleDispatchServices(bool toggle)
        {
            if (ENABLE_STANDARD_SPAWNS) return;
            Function.Call(Hash.ENABLE_DISPATCH_SERVICE, 1, toggle); // DT_PoliceAutomobile
            if (!ENABLE_POLICE_HELICOPTER)
            {
                Function.Call(Hash.ENABLE_DISPATCH_SERVICE, 2, toggle); // DT_PoliceHelicopter
            }
            Function.Call(Hash.ENABLE_DISPATCH_SERVICE, 4, toggle); // DT_SwatAutomobile
            Function.Call(Hash.ENABLE_DISPATCH_SERVICE, 6, toggle); // DT_PoliceRiders
            //Function.Call(Hash.ENABLE_DISPATCH_SERVICE, 8, toggle); // DT_PoliceRoadblock
        }
    }
}