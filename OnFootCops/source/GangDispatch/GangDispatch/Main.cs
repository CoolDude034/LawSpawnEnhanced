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

        Random random = new Random();
        Ped sniper;
        bool isSniperSpawned = false;

        WeaponHash[] UNIT_WEAPONS = { WeaponHash.SMG, WeaponHash.CarbineRifle, WeaponHash.PumpShotgun };
        int MAX_UNITS = 8;
        float MIN_POLICE_SPAWN_DISTANCE = 100f;
        float MIN_DISTANCE_FROM_SNIPER_SPAWNS = 500f;

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
        };

        Ped SpawnUnit(Model model, Vector3 pos, Model wpn)
        {
            var ped = World.CreatePed(model, pos);
            ped.AlwaysKeepTask = true;
            ped.Task.FightAgainst(Game.Player.Character);

            Function.Call(Hash.GIVE_WEAPON_TO_PED, ped, wpn, 9999, false, true);
            Function.Call(Hash.SET_PED_PROP_INDEX, ped, 0, 2, 0, true);
            Function.Call(Hash.SET_PED_COMBAT_MOVEMENT, ped, 2);
            Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 46); // BF_AlwaysFight

            groups.Add(ped);

            return ped;
        }

        void SpawnSniper(Vector3 pos)
        {
            if (isSniperSpawned) return;

            sniper = World.CreatePed(PedHash.Swat01SMY, pos);
            sniper.AlwaysKeepTask = true;
            sniper.Task.FightAgainst(Game.Player.Character);

            Function.Call(Hash.GIVE_WEAPON_TO_PED, sniper, WeaponHash.SniperRifle, 9999, true, true);
            Function.Call(Hash.SET_PED_COMBAT_MOVEMENT, sniper, 0);
            Function.Call(Hash.SET_PED_PROP_INDEX, sniper, 0, 2, 0, true);
            Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, sniper, 46); // BF_AlwaysFight


            isSniperSpawned = true;
        }

        public Main()
        {
            Tick += OnTick;
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
            foreach (Vector3 pos in SniperSpawns)
            {
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
            Vector3[] randomPos = { Game.Player.Character.ForwardVector * MIN_POLICE_SPAWN_DISTANCE, Game.Player.Character.ForwardVector * -MIN_POLICE_SPAWN_DISTANCE, Game.Player.Character.RightVector * MIN_POLICE_SPAWN_DISTANCE, Game.Player.Character.RightVector * -MIN_POLICE_SPAWN_DISTANCE };
            Vector3 newPos = randomPos[random.Next(0, randomPos.Length)];
            Vector3 pos = World.GetSafeCoordForPed(Game.Player.Character.Position + newPos);

            return pos;
        }

        void UpdateState()
        {
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

        void UpdateGroups()
        {
            if (Game.Player.WantedLevel >= 3 && canSpawn())
            {
                // Function.Call<bool>(Hash.IS_INTERIOR_SCENE)
                bool IS_SEEN_BY_COPS = Function.Call<bool>(Hash.IS_WANTED_AND_HAS_BEEN_SEEN_BY_COPS, Game.Player);
                if (Game.Player.Character.IsOnFoot && IS_SEEN_BY_COPS)
                {
                    var pos = FindAvailableSpawnPoint();
                    if (pos != Vector3.Zero)
                    {
                        SpawnUnit(PedHash.Swat01SMY, pos, UNIT_WEAPONS[random.Next(0, UNIT_WEAPONS.Length)]);
                    }
                }
            }
        }

        void UpdateExistingMembers()
        {

            if (sniper != null && sniper.Exists())
            {
                if (Game.Player.Character.Position.DistanceTo(sniper.Position) > MIN_DISTANCE_FROM_SNIPER_SPAWNS || Game.Player.WantedLevel <= 0 || sniper.IsDead)
                {
                    sniper.MarkAsNoLongerNeeded();
                    sniper = null;
                    isSniperSpawned = false;
                }
            }
            else
            {
                if (Game.Player.WantedLevel >= 4 && isInSniperLocation())
                {
                    var pos = FindNearestSniperSpawn();

                    if (pos != Vector3.Zero)
                    {
                        SpawnSniper(pos);
                    }
                }
            }

            if (groups.Count > 0)
            {
                for (int i = groups.Count - 1; i > -1; i--)
                {
                    var ped = groups[i];
                    if (ped != null && ped.Exists())
                    {
                        if (ped.IsDead || Game.Player.WantedLevel <= 0 || Game.Player.IsDead)
                        {
                            ped.MarkAsNoLongerNeeded();
                            groups.RemoveAt(i);
                        }
                    }
                }

            }
        }

        void OnTick(object sender, EventArgs e)
        {
            UpdateState();
            UpdateGroups();
            UpdateExistingMembers();
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
