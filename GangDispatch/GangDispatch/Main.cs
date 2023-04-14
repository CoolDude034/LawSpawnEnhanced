﻿using System;
using System.Collections.Generic;
using GTA;
using GTA.Math;
using GTA.Native;

namespace GangDispatch
{
    public class Main : Script
    {

        List<Ped> groups = new List<Ped>();

        ScriptSettings Config;
        Random random = new Random();
        Ped sniper;
        bool isSniperSpawned = false;

        // Models
        Model[] assault_weapons = { WeaponHash.SMG, WeaponHash.CarbineRifle, WeaponHash.PumpShotgun };

        // Private variables
        int MAX_UNITS;
        int MAX_WANTED_LEVEL;
        int TIME_BETWEEN_SPAWNS; // now a intvalue
        float MIN_POLICE_SPAWN_DISTANCE;
        float MIN_DISTANCE_FROM_SNIPER_SPAWNS;
        float MIN_POLICE_DESPAWN_RANGE;
        float COP_SEARCH_DISTANCE;
        bool ENABLE_STANDARD_SPAWNS;
        bool ASSAULT_FORCE_KNOWS_WHERE_YOU_ARE;
        string COP_MODEL_OVERRIDE;
        string COP_COUNTRY_MODEL_OVERRIDE;
        string ARMY_MODEL_OVERRIDE;

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
        };

        // There was no reason for this to return anything, so just return void
        void SpawnUnit(Vector3 pos)
        {
            var model = GetModelByZone();
            if (!model.IsLoaded)
            {
                model.Request();
            }

            if (model.IsInCdImage && model.IsValid)
            {
                var ped = World.CreatePed(model, pos);
                if (ASSAULT_FORCE_KNOWS_WHERE_YOU_ARE)
                {
                    ped.Task.FightAgainst(Game.Player.Character);
                }
                else
                {
                    // Search the player's current location.
                    ped.Task.GoStraightTo(Game.Player.Character.Position.Around(COP_SEARCH_DISTANCE));
                }

                var weapon = assault_weapons[random.Next(0, assault_weapons.Length)];
                ped.Weapons.Give(weapon, 9999, true, true);

                Function.Call(Hash.SET_PED_PROP_INDEX, ped, 0, 1, 0, true);
                Function.Call(Hash.SET_PED_COMBAT_MOVEMENT, ped, 2); // stationary
                Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 46, true); // BF_AlwaysFight
                Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 21, true); // chase target onfoot
                Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 22, true); // drag injured *comrades* to safety

                groups.Add(ped);
            }
        }

        void SpawnSniper(Vector3 pos)
        {
            if (isSniperSpawned) return;

            var model = GetModelByZone();
            if (!model.IsLoaded)
            {
                model.Request();
            }

            if (model.IsInCdImage && model.IsValid)
            {
                sniper = World.CreatePed(model, pos);
                sniper.AlwaysKeepTask = true;
                sniper.Task.FightAgainst(Game.Player.Character);

                sniper.Weapons.Give(WeaponHash.SniperRifle, 9999, true, true);

                Function.Call(Hash.SET_PED_COMBAT_MOVEMENT, sniper, 0);
                Function.Call(Hash.SET_PED_PROP_INDEX, sniper, 0, 1, 0, true);
                Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, sniper, 46, true); // BF_AlwaysFight
                Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, sniper, 21, false); // DONT chase target onfoot
                Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, sniper, 22, false); // DONT drag injured *comrades* to safety

                isSniperSpawned = true;
            }
        }

        public Main()
        {
            Config = ScriptSettings.Load("scripts/" + this.Filename + ".ini");
            MAX_UNITS = Settings.GetValue<int>("SETTINGS", "MAX_UNITS", 8);
            MAX_WANTED_LEVEL = Settings.GetValue<int>("SETTINGS", "MAX_WANTED_LEVEL", 3);
            MIN_POLICE_SPAWN_DISTANCE = Settings.GetValue<float>("SETTINGS", "MIN_POLICE_SPAWN_DISTANCE", 100f);
            MIN_DISTANCE_FROM_SNIPER_SPAWNS = Settings.GetValue<float>("SETTINGS", "MIN_DISTANCE_FROM_SNIPER_SPAWNS", 500f);
            MIN_POLICE_DESPAWN_RANGE = Settings.GetValue<float>("SETTINGS", "MIN_POLICE_DESPAWN_RANGE", 600f);
            TIME_BETWEEN_SPAWNS = Settings.GetValue<int>("SETTINGS", "TIME_BETWEEN_SPAWNS", 6000);
            COP_SEARCH_DISTANCE = Settings.GetValue<float>("SETTINGS", "COP_SEARCH_DISTANCE", 400f);
            ENABLE_STANDARD_SPAWNS = Settings.GetValue<bool>("SETTINGS", "ENABLE_STANDARD_SPAWNS", false);
            ASSAULT_FORCE_KNOWS_WHERE_YOU_ARE = Settings.GetValue<bool>("SETTINGS", "ASSAULT_FORCE_KNOWS_WHERE_YOU_ARE", true);
            // Updated code to look for modelnames instead of pedhashes, note this means that you need to update your config otherwise it will fallback to swat
            // civmale/civfemale pedtypes can cause in-fighting between them
            COP_MODEL_OVERRIDE = Settings.GetValue<string>("SETTINGS", "COP_MODEL_OVERRIDE", "s_m_y_swat_01");
            ARMY_MODEL_OVERRIDE = Settings.GetValue<string>("SETTINGS", "ARMY_MODEL_OVERRIDE", "s_m_y_marine_03");
            COP_COUNTRY_MODEL_OVERRIDE = Settings.GetValue<string>("SETTINGS", "COP_COUNTRY_MODEL_OVERRIDE", "s_m_y_swat_01");

            Tick += OnTick;
            Aborted += ScriptCleanup;
        }

        bool canSpawn()
        {
            if (groups.Count >= MAX_UNITS)
            {
                return false;
            }

            return true;
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
            // Function.Call<bool>(Hash.IS_INTERIOR_SCENE);
            bool isInsideInterior = Function.Call<bool>(Hash.IS_INTERIOR_SCENE);
            Vector3[] randomPos = { Game.Player.Character.ForwardVector * MIN_POLICE_SPAWN_DISTANCE, Game.Player.Character.ForwardVector * -MIN_POLICE_SPAWN_DISTANCE, Game.Player.Character.RightVector * MIN_POLICE_SPAWN_DISTANCE, Game.Player.Character.RightVector * -MIN_POLICE_SPAWN_DISTANCE };
            Vector3 newPos = randomPos[random.Next(0, randomPos.Length)];
            Vector3 pos = World.GetSafeCoordForPed(Game.Player.Character.Position + newPos, sidewalk: !isInsideInterior);

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

            return new Model(COP_MODEL_OVERRIDE);
        }

        string GetZoneType()
        {
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ArmyB") || Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zancudo"))
            {
                return "ARMY";
            }
            else if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Desrt") || Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Alamo") || Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Lago") || Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Slab") || Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Paleto"))
            {
                return "DESERT";
            }

            return "GENERAL";
        }

        void UpdateState()
        {
            if (ENABLE_STANDARD_SPAWNS) return;
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
            if (Game.Player.WantedLevel >= MAX_WANTED_LEVEL && canSpawn())
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
                            ped.MarkAsNoLongerNeeded();
                            groups.RemoveAt(i);
                        }
                        else
                        {
                            if (ped.IsDead || Game.Player.WantedLevel <= 0 || Game.Player.IsDead || World.GetDistance(ped.Position, Game.Player.Character.Position) > MIN_POLICE_DESPAWN_RANGE)
                            {
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

        void OnTick(object sender, EventArgs e)
        {
            UpdateState();
            UpdateGroups();
            UpdateExistingMembers();
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

            ClearAllAssaultingMembers(true);
            ToggleDispatchServices(true);
        }

        void ToggleDispatchServices(bool toggle)
        {
            Function.Call(Hash.ENABLE_DISPATCH_SERVICE, 1, toggle); // DT_PoliceAutomobile
            Function.Call(Hash.ENABLE_DISPATCH_SERVICE, 2, toggle); // DT_PoliceHelicopter
            Function.Call(Hash.ENABLE_DISPATCH_SERVICE, 4, toggle); // DT_SwatAutomobile
            Function.Call(Hash.ENABLE_DISPATCH_SERVICE, 6, toggle); // DT_PoliceRiders
            Function.Call(Hash.ENABLE_DISPATCH_SERVICE, 8, toggle); // DT_PoliceRoadblock
        }
    }
}
