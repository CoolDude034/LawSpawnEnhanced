using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using GTA;
using GTA.Math;
using GTA.Native;

namespace GangDispatch
{
    public class Main : Script
    {

        List<Ped> groups = new List<Ped>();
        Dictionary<int, Prop> shields = new Dictionary<int, Prop>();
        Dictionary<int, Ped> heavies = new Dictionary<int, Ped>();
        Dictionary<int, Ped> rescue_hrt = new Dictionary<int, Ped>();

        List<Blip> map_markers = new List<Blip>();

        public enum FormationType // https://docs.fivem.net/natives/?_0xCE2F5FC3AF7E8C1E
        {
            Default = 0,
            CircleAroundLeader,
            CircleAroundLeaderAlt,
            LineAroundLeader,
            ArrowFormation,
            VFormation,
            LineFollowFormation,
            SingleFormation,
            Pairwise
        }

        Random random = new Random();
        Ped sniper;
        bool isSniperSpawned = false;
        bool isPoliceBesiegeSpawned = false;
        bool canSpawnPeds = false;
        bool underAssault = false;
        Vector3 policeBesiegeLocation = new Vector3(204.728027f, 201.1926f, 104.5698f);
        float m_fTimeUntilNextWave;
        bool m_bIsInitialFirstTime;

        // Models
        Model[] assault_weapons = { WeaponHash.SMG, WeaponHash.CarbineRifle, WeaponHash.PumpShotgun, WeaponHash.AssaultShotgun };

        // Private variables
        int MAX_UNITS;
        int MAX_WANTED_LEVEL;
        int MAX_SHIELDS;
        int MAX_HEAVIES;
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
        string ARMY_MODEL_OVERRIDE;
        string LC_MODEL_OVERRIDE;
        string PALETO_MODEL_OVERRIDE;
        string PMC_MODEL_OVERRIDE;
        string SHIELD_MODEL;
        string HEAVY_UNIT_MODEL_SWAT;
        string HEAVY_UNIT_MODEL_PMC;
        string HEAVY_UNIT_MODEL_ARMY;

        bool OLD_SPAWNING_SYSTEM;
        int NUM_PER_SQUAD;
        int SHIELD_SPAWN_CHANCE_WL3;
        int SHIELD_SPAWN_CHANCE_WL4;
        int SHIELD_SPAWN_CHANCE_WL5;
        int HEAVY_SPAWN_CHANCE_WL3;
        int HEAVY_SPAWN_CHANCE_WL4;
        int HEAVY_SPAWN_CHANCE_WL5;
        FormationType SQUAD_FORMATION_TYPE;

        float GRACE_PERIOD;
        float ASSAULT_DURATION;

        // Regional Shields
        bool USE_REGIONAL_SHIELDS;
        string SWAT_SHIELD_MODEL;
        string MWR_SHIELD_MODEL;
        string ARMY_SHIELD_MODEL;
        string LC_SHIELD_MODEL;
        string PALETO_SHIELD_MODEL;

        string LOADOUT_SET;
        string HEAVY_UNIT_LOADOUT_OVERRIDE;

        // Wanted Level Specific Overrides
        bool ENABLE_WANTED_LEVEL_SPECIFIC_OVERRIDES;
        string COP_WL3;
        string COP_WL4;
        string COP_WL5;
        string FORCED_SNIPER_MODEL;

        bool SPAWN_EVERYWHERE;
        bool DISABLE_RANDOM_PROPS;
        bool DISABLE_COMPONENTS;
        bool DISABLE_RAMPAGES;

        int CHANCE_FOR_RESCUE_TEAM;

        int ARMOR_LEVEL_1;
        int ARMOR_LEVEL_2;
        int ARMOR_LEVEL_3;

        int NUM_CUSTOM_ZONES;

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

        // Rampages
        bool m_bIsRampageActive = false;
        float m_fTimeUntilRampageOver;
        Vector3 pillboxHillLocation = new Vector3(135.093826f, -714.6967f, 46.07659f);
        Vector3 missionRowLocation = new Vector3(424.004181f, -981.9115f, 29.7113838f);
        Vector3 blitzPlayLoc = new Vector3(880.109558f, -2351.15771f, 29.3310966f);
        Vector3 merryweatherDocks = new Vector3(602.3549f, -3148.63672f, 5.06925726f);

        void AddHelmet(Ped ped)
        {
            if (DISABLE_RANDOM_PROPS) return;
            if (ped.Model == "s_m_y_swat_01")
            {
                Function.Call(Hash.SET_PED_PROP_INDEX, ped, 0, 0, 0, true);
            }
            else if (ped.Model == "U_M_M_Juggernaut_03")
            {
                Function.Call(Hash.SET_PED_PROP_INDEX, ped, 1, random.Next(0,1), 0, true); // p_eyes
                Function.Call(Hash.SET_PED_PROP_INDEX, ped, 0, 0, 0, true); // p_head
            }
            else
            {
                Function.Call(Hash.SET_PED_RANDOM_PROPS, ped);
            }
        }

        void SetComponent(Ped ped)
        {
            if (DISABLE_COMPONENTS) return;
            if (ped.Model == "s_m_y_swat_01")
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
            else if (ped.Model == "s_m_y_cop_01")
            {
                Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped, 8, 2, 0, 0); // PV_COMP_ACCS
                Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped, 9, 2, 0, 0); // PV_COMP_TASK
            }
            else if (ped.Model == "s_m_y_sheriff_01")
            {
                Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped, 9, 2, 0, 0); // PV_COMP_TASK
            }
            else
            {
                Function.Call(Hash.SET_PED_RANDOM_COMPONENT_VARIATION, ped, 0);
            }
        }

        int GetArmorValue()
        {
            if (Game.Player.WantedLevel == 3)
                return DIFFICULTY * ARMOR_LEVEL_1;
            if (Game.Player.WantedLevel == 4)
                return DIFFICULTY * ARMOR_LEVEL_2;
            if (Game.Player.WantedLevel == 5)
                return DIFFICULTY * ARMOR_LEVEL_3;
            return 0;
        }

        int GetShieldSpawnChance()
        {
            if (Game.Player.WantedLevel == 3)
                return SHIELD_SPAWN_CHANCE_WL3;
            if (Game.Player.WantedLevel == 4)
                return SHIELD_SPAWN_CHANCE_WL4;
            if (Game.Player.WantedLevel == 5)
                return SHIELD_SPAWN_CHANCE_WL5;
            return 0;
        }

        int GetHeavySpawnChance()
        {
            if (Game.Player.WantedLevel == 3)
                return HEAVY_SPAWN_CHANCE_WL3;
            if (Game.Player.WantedLevel == 4)
                return HEAVY_SPAWN_CHANCE_WL4;
            if (Game.Player.WantedLevel == 5)
                return HEAVY_SPAWN_CHANCE_WL5;
            return 0;
        }

        bool IsNotLaw(Ped ped)
        {
            if (Function.Call<int>(Hash.GET_PED_TYPE, ped) == 6)
                return false;
            if (Function.Call<int>(Hash.GET_PED_TYPE, ped) == 27)
                return false;
            if (Function.Call<int>(Hash.GET_PED_TYPE, ped) == 29)
                return false;
            return true;
        }

        bool SpawnUnit(Vector3 pos, int group = 0, bool isLeader = false, bool isHeavyUnit = false)
        {
            var model = isHeavyUnit ? GetHeavyUnitModelByZone() : GetModelByZone();
            if (!model.IsLoaded)
            {
                model.Request(5);
            }

            if (model.IsInCdImage && model.IsValid)
            {
                var ped = World.CreatePed(model, pos);
                ped.Task.Combat(Game.Player.Character, TaskCombatFlags.ArrestTarget);

                if (!isHeavyUnit)
                {
                    if (ENABLE_OLD_WEAPON_SYSTEM)
                    {
                        var weapon = assault_weapons[random.Next(0, assault_weapons.Length)];
                        ped.Weapons.Give(weapon, 9999, true, true);
                        ped.Weapons.Current.InfiniteAmmo = true;
                    }
                    else
                    {
                        var hash = Function.Call<int>(Hash.GET_HASH_KEY, LOADOUT_SET);
                        Function.Call(Hash.GIVE_LOADOUT_TO_PED, ped, hash);
                    }

                    if (DIFFICULTY != 0)
                    {
                        ped.Armor += GetArmorValue();
                    }

                    AddHelmet(ped);
                    SetComponent(ped);
                    // Adjust Combat Flags (See: https://docs.fivem.net/natives/?_0x9F7794730795E019)
                    Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 60, true); // allow smoke grenade throwing
                    Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 22, true); // allow dragging allies
                    Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 21, true); // allow chasing target onfoot
                    Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 41, true); // allow commandering vehicles
                    Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 42, true); // allow flanking
                    Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 50, true); // allow charging
                    Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 28, true); // If we don't have cover and can't see our target it's possible we will advance, even if the target is in cover
                    Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 29, true); // This will have the ped move to defensive areas and within attack windows before performing the cover search
                    Function.Call(Hash.SET_PED_COMBAT_MOVEMENT, ped, 2); // CM_WillAdvance
                }
                else
                {
                    ped.Health = 2000;
                    ped.Armor = 100;
                    ped.CanWrithe = false;
                    ped.CanRagdoll = false;
                    ped.CanSufferCriticalHits = false;
                    ped.Voice = "S_M_Y_BLACKOPS_01_R2PVG";

                    Blip blipHeavy = ped.AddBlip();
                    blipHeavy.Alpha = 255;
                    blipHeavy.Sprite = BlipSprite.Rampage;
                    blipHeavy.Color = BlipColor.Red;
                    blipHeavy.Name = "Heavy Unit";
                    blipHeavy.Scale = 0.6f;

                    if (HEAVY_UNIT_LOADOUT_OVERRIDE != "" && HEAVY_UNIT_LOADOUT_OVERRIDE != null)
                    {
                        var hash = Function.Call<int>(Hash.GET_HASH_KEY, HEAVY_UNIT_LOADOUT_OVERRIDE);
                        Function.Call(Hash.GIVE_LOADOUT_TO_PED, ped, hash);
                    }
                    else
                    {
                        if (random.Next(1, 100) == 25)
                        {
                            ped.FiringPattern = FiringPattern.FullAuto;
                            ped.Weapons.Give(WeaponHash.CombatMG, 9999, true, true);
                            ped.Weapons.Current.InfiniteAmmo = true;
                            Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatMG, WeaponComponentHash.AtScopeMedium);
                        }
                        else if (random.Next(1, 100) == 25)
                        {
                            ped.FiringPattern = FiringPattern.FullAuto;
                            ped.Weapons.Give(WeaponHash.CombatMGMk2, 9999, true, true);
                            ped.Weapons.Current.InfiniteAmmo = true;
                            Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatMGMk2, WeaponComponentHash.AtScopeMedium);
                            Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatMGMk2, WeaponComponentHash.CombatMGMk2Camo04);
                            Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatMGMk2, WeaponComponentHash.CombatMGMk2ClipArmorPiercing);
                            Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, ped, WeaponHash.CombatMGMk2, WeaponComponentHash.CombatMGMk2ClipTracer);
                        }
                        else if (random.Next(1, 100) == 35)
                        {
                            ped.FiringPattern = FiringPattern.BurstFire;
                            ped.Weapons.Give(WeaponHash.PumpShotgun, 9999, true, true);
                            ped.Weapons.Current.InfiniteAmmo = true;
                        }
                        else
                        {
                            ped.FiringPattern = FiringPattern.FullAuto;
                            ped.Weapons.Give(WeaponHash.AssaultShotgun, 9999, true, true);
                            ped.Weapons.Current.InfiniteAmmo = true;
                        }
                    }

                    if (ped.Model == "hc_gunman")
                    {
                        Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped, 8, 2, 0, 0); // accs
                        Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped, 1, 1, 0, 0); // berd
                        Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped, 10, 1, 0, 0); // decl
                        Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped, 6, 1, 0, 0); // feet
                        Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped, 2, random.Next(0, 1), 0, 0); // hair
                        Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped, 0, random.Next(0, 1), 0, 0); // head
                        Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped, 3, 1, 0, 0); // uppr
                        Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped, 4, 1, 0, 0); // lowr
                        Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped, 9, 1, 0, 0); // task
                    }

                    // Adjust Combat Flags (See: https://docs.fivem.net/natives/?_0x9F7794730795E019)
                    Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 60, false); // allow smoke grenade throwing
                    Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 22, false); // allow dragging allies
                    Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 21, false); // allow chasing target onfoot
                    Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 41, false); // allow commandering vehicles
                    Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 42, true); // allow flanking
                    Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 50, true); // allow charging
                    Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 39, false); // prevent busting
                    Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 38, true); // disable bullet reactions
                    Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 30, true); // allow shooting even if we don't have LOS
                    Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 29, true); // This will have the ped move to defensive areas and within attack windows before performing the cover search
                    Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 28, true); // If we don't have cover and can't see our target it's possible we will advance, even if the target is in cover
                    Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 26, true); // disable flinching
                    Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 23, false); // require LOS to shoot
                    Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 0, false); // disable cover
                    Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 62, true); // Will clear a set defensive area if that area cannot be reached
                    Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 71, true); // Permits ped to charge a target outside the assigned defensive area.
                    Function.Call(Hash.SET_PED_COMBAT_MOVEMENT, ped, 2); // CM_WillAdvance

                    heavies.Add(ped.Handle, ped);
                }

                ped.RelationshipGroup = RelationshipGroupHash.Cop;
                ped.CombatAbility = CombatAbility.Professional;
                if (IsNotLaw(ped))
                {
                    Function.Call(Hash.SET_PED_AS_COP, ped, true);
                }
                Function.Call(Hash.SET_PED_HAS_AI_BLIP, ped, false);
                Function.Call(Hash.SET_PED_AI_BLIP_FORCED_ON, ped, true);

                if (Function.Call<bool>(Hash.DOES_GROUP_EXIST, group))
                {
                    ped.NeverLeavesGroup = true;
                    if (isLeader)
                    {
                        Function.Call(Hash.SET_PED_AS_GROUP_LEADER, ped, group);
                    }
                    else
                    {
                        Function.Call(Hash.SET_PED_AS_GROUP_MEMBER, ped, group);
                    }
                }

                groups.Add(ped);
                model.MarkAsNoLongerNeeded();
                return true;
            }

            return false;
        }

        bool SpawnShield(Vector3 pos, int group = 0, bool isLeader = false)
        {
            var model = GetModelByZone();
            if (!model.IsLoaded)
            {
                model.Request(5);
            }
            var shieldModel = USE_REGIONAL_SHIELDS ? GetShieldModelByZone() : new Model(SHIELD_MODEL);
            if (!shieldModel.IsLoaded)
            {
                shieldModel.Request(5);
            }

            if (model.IsInCdImage && model.IsValid && shieldModel.IsInCdImage && shieldModel.IsValid)
            {
                var ped = World.CreatePed(model, pos);
                var shield = World.CreateProp(shieldModel, ped.Position.Around(5f), true, true);
                shield.IsPersistent = true;
                shield.IsInvincible = true;
                shield.LodDistance = 100;
                shield.SetNoCollision(ped, false);
                Attach(ped, shield);
                if (DIFFICULTY == 3)
                {
                    ped.Weapons.Give(WeaponHash.MicroSMG, 9999, true, true);
                    ped.Weapons.Current.InfiniteAmmo = true;
                }
                else
                {
                    ped.Weapons.Give(WeaponHash.Pistol, 9999, true, true);
                    ped.Weapons.Current.InfiniteAmmo = true;
                }
                ped.Task.Combat(Game.Player.Character);

                DoAnims(ped, shield);

                ped.IsFireProof = true;
                ped.IsMeleeProof = true;
                ped.CanWrithe = false;
                ped.CanSufferCriticalHits = false;
                ped.RelationshipGroup = RelationshipGroupHash.Cop;
                ped.CombatAbility = CombatAbility.Professional;
                if (IsNotLaw(ped))
                {
                    Function.Call(Hash.SET_PED_AS_COP, ped, true);
                }
                Function.Call(Hash.SET_PED_HAS_AI_BLIP, ped, false);
                Function.Call(Hash.SET_PED_AI_BLIP_FORCED_ON, ped, true);

                if (DIFFICULTY != 0)
                {
                    ped.Armor += GetArmorValue();
                }

                if (Function.Call<bool>(Hash.DOES_GROUP_EXIST, group))
                {
                    ped.NeverLeavesGroup = true;
                    if (isLeader)
                    {
                        Function.Call(Hash.SET_PED_AS_GROUP_LEADER, ped, group);
                    }
                    else
                    {
                        Function.Call(Hash.SET_PED_AS_GROUP_MEMBER, ped, group);
                    }
                }

                AddHelmet(ped);
                SetComponent(ped);
                // Adjust Combat Flags (See: https://docs.fivem.net/natives/?_0x9F7794730795E019)
                Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 60, false); // prevent smoke grenade throwing
                Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 22, false); // prevent dragging allies (can't do that when carrying a shield :P)
                Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 21, false); // prevent chasing target onfoot
                Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 41, false); // prevent commandering vehicles
                Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 42, true); // allow flanking
                Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 50, true); // allow charging
                Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 45, false); // prevent clearing their defensive area when they reach it
                Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 39, false); // prevent busting (can't do that when you have a shield :P)
                Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 37, true); // When defensive area is reached the area is cleared and the ped is set to use defensive combat movement
                Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 38, true); // disable bullet reactions
                Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 30, true); // allow shooting even if we don't have LOS
                Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 29, true); // This will have the ped move to defensive areas and within attack windows before performing the cover search
                Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 28, true); // If we don't have cover and can't see our target it's possible we will advance, even if the target is in cover
                Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 26, true); // disable flinching
                Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 23, true); // require LOS to shoot
                Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 0, false); // disable cover
                Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 62, true); // Will clear a set defensive area if that area cannot be reached
                Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 71, true); // Permits ped to charge a target outside the assigned defensive area.
                Function.Call(Hash.SET_PED_COMBAT_MOVEMENT, ped, 2); // CM_WillAdvance

                shields.Add(ped.Handle, shield);
                groups.Add(ped);

                shieldModel.MarkAsNoLongerNeeded();
                model.MarkAsNoLongerNeeded();
                return true;
            }

            return false;
        }

        void Attach(Ped ped, Prop shield)
        {
            Vector3[] pos = GetShieldPosition(ped, shield);
            int bone = GetBone(ped, shield);
            Function.Call(Hash.ATTACH_ENTITY_BONE_TO_ENTITY_BONE, new InputArgument[]
            {
                shield,
                ped,
                bone,
                pos[0].X,
                pos[0].Y,
                pos[0].Z,
                pos[1].X,
                pos[1].Y,
                pos[1].Z,
                false,
                false,
                false,
                false,
                2,
                true
            });
            Function.Call(Hash.FIX_OBJECT_FRAGMENT, shield);
        }

        void DetachShield(Ped ped, Prop shield)
        {
            CancelAnimation(ped, shield);
            Function.Call(Hash.DETACH_ENTITY, new InputArgument[] { shield, true, true });
			Function.Call(Hash.FIX_OBJECT_FRAGMENT, shield);
            shield.MarkAsNoLongerNeeded();
        }

        void DestroyShield(Ped ped, Prop shield)
        {
            CancelAnimation(ped, shield);
            Function.Call(Hash.DETACH_ENTITY, new InputArgument[] { shield, true, true });
			Function.Call(Hash.FIX_OBJECT_FRAGMENT, shield);
            shield.Delete();
        }

        void CancelAnimation(Ped ped, Prop shield)
        {
            var animPlaying = Function.Call<bool>(Hash.IS_ENTITY_PLAYING_ANIM, new InputArgument[]
            {
                ped,
                "weapons@pistol_1h@gang",
                "aim_med_loop",
                3
            });

            if (animPlaying)
            {
                ped.Task.ClearAnimation("weapons@pistol_1h@gang", "aim_med_loop");
            }
        }

        void DoAnims(Ped ped, Prop shield)
        {
            var wpnCheck = ped.Weapons.Current.Group == WeaponGroup.Pistol || ped.Weapons.Current.Hash == WeaponHash.MicroSMG;
            if (wpnCheck)
            {
                Function.Call(Hash.SET_PED_CAN_ARM_IK, new InputArgument[]
                {
                    ped,
                    true
                });
                var bool1 = !ped.IsJumping && ped.IsFalling && !ped.IsRagdoll;
                if (bool1)
                {
                    var v1 = Function.Call<bool>(Hash.IS_ENTITY_PLAYING_ANIM, new InputArgument[]
                    {
                        ped,
                        "weapons@pistol_1h@gang",
                        "aim_med_loop",
                        3
                    });
                    if (v1)
                    {
                        ped.Task.PlayAnimation("weapons@pistol_1h@gang", "aim_med_loop");
                    }
                    else
                    {
                        var v2 = ped.IsReloading || ped.IsJumping || ped.IsFalling || ped.IsRagdoll;
                        if (v2)
                        {
                            ped.Task.ClearAnimation("weapons@pistol_1h@gang", "aim_med_loop");
                        }
                        Function.Call(Hash.SET_PED_CAN_ARM_IK, new InputArgument[]
                        {
                            ped,
                            false
                        });
                    }
                }
            }
        }

        int GetBone(Ped ped, Prop shield)
        {
            if (ped.IsReloading)
            {
                return Function.Call<int>(Hash.GET_PED_BONE_INDEX, new InputArgument[] { ped, 5232 });
            }

            return Function.Call<int>(Hash.GET_PED_BONE_INDEX, new InputArgument[] { ped, 36029 });
        }

        Vector3[] GetShieldPosition(Ped ped, Prop shield)
        {
            Vector3[] shield_pos = new Vector3[]
            {
                Vector3.Zero,
                Vector3.Zero
            };

            if (ped.IsReloading)
            {
                shield_pos[0] = new Vector3(0.500001f, 0.045f, -0.04f);
                shield_pos[1] = new Vector3(-248.81f, 8.92f, -126.71f);
            }
            else
            {
                shield_pos[0] = new Vector3(0f, -0.0700002f, 0f);
                shield_pos[1] = new Vector3(39f, 188.5f, 4f);
            }

            return shield_pos;
        }

        // meet the sniper
        void SpawnSniper(Vector3 pos)
        {
            if (isSniperSpawned) return;

            var model = GetModelByZone();
            if (FORCED_SNIPER_MODEL != "" && FORCED_SNIPER_MODEL != null)
            {
                model = new Model(FORCED_SNIPER_MODEL);
            }
            if (!model.IsLoaded)
            {
                model.Request(5);
            }

            if (model.IsInCdImage && model.IsValid)
            {
                sniper = World.CreatePed(model, pos);
                sniper.CanWrithe = false;
                if (random.Next(1, 100) == 25)
                {
                    sniper.Weapons.Give(WeaponHash.MarksmanRifleMk2, 9999, true, true);
                    sniper.Weapons.Current.InfiniteAmmo = true;
                    Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, sniper, WeaponHash.SniperRifle, WeaponComponentHash.MarksmanRifleMk2ClipTracer);
                }
                else
                {
                    sniper.Weapons.Give(WeaponHash.SniperRifle, 9999, true, true);
                    sniper.Weapons.Current.InfiniteAmmo = true;
                }
                sniper.Task.Combat(Game.Player.Character, TaskCombatFlags.UseSniperAimIntro);

                sniper.RelationshipGroup = RelationshipGroupHash.Cop;
                sniper.CombatAbility = CombatAbility.Professional;
                sniper.Voice = "SILENT_PVG";

                if (IsNotLaw(sniper))
                {
                    Function.Call(Hash.SET_PED_AS_COP, sniper, true);
                }

                sniper.SeeingRange = 1000.0f;

                AddHelmet(sniper);
                SetComponent(sniper);
                Function.Call(Hash.SET_PED_COMBAT_MOVEMENT, sniper, 0);
                Function.Call(Hash.SET_PED_COMBAT_RANGE, sniper, 3);
                Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, sniper, 46, true); // BF_AlwaysFight
                Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, sniper, 21, false); // DONT chase target onfoot
                Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, sniper, 22, false); // DONT drag injured *comrades* to safety
                Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, sniper, 27, true);
                Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, sniper, 60, false); // prevent throwing smoke grenades
                Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, sniper, 0, false); // disable taking cover
                Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, sniper, 39, false); // prevent busting

                isSniperSpawned = true;
                model.MarkAsNoLongerNeeded();
            }
        }

        public Main()
        {
            ScriptSettings.Load("scripts/" + this.Filename + ".ini");
            MAX_UNITS = Settings.GetValue<int>("SETTINGS", "MAX_UNITS", 8);
            MAX_WANTED_LEVEL = Settings.GetValue<int>("SETTINGS", "MAX_WANTED_LEVEL", 4);
            MAX_SHIELDS = Settings.GetValue<int>("SETTINGS", "MAX_SHIELDS", 2);
            MAX_HEAVIES = Settings.GetValue<int>("SETTINGS", "MAX_HEAVIES", 1);
            MIN_POLICE_SPAWN_DISTANCE = Settings.GetValue<float>("SETTINGS", "MIN_POLICE_SPAWN_DISTANCE", 150f);
            MIN_DISTANCE_FROM_SNIPER_SPAWNS = Settings.GetValue<float>("SETTINGS", "MIN_DISTANCE_FROM_SNIPER_SPAWNS", 500f);
            MIN_POLICE_DESPAWN_RANGE = Settings.GetValue<float>("SETTINGS", "MIN_POLICE_DESPAWN_RANGE", 600f);
            TIME_BETWEEN_SPAWNS = Settings.GetValue<int>("SETTINGS", "TIME_BETWEEN_SPAWNS", 80);
            ENABLE_STANDARD_SPAWNS = Settings.GetValue<bool>("SETTINGS", "ENABLE_STANDARD_SPAWNS", false);
            ENABLE_POLICE_HELICOPTER = Settings.GetValue<bool>("SETTINGS", "ENABLE_POLICE_HELICOPTER", false);
            ENABLE_POLICE_BESIEGE = Settings.GetValue<bool>("SETTINGS", "ENABLE_POLICE_BESIEGE", false); // defaulting this to off
            ENABLE_OLD_WEAPON_SYSTEM = Settings.GetValue<bool>("SETTINGS", "ENABLE_OLD_WEAPON_SYSTEM", false);
            MIN_DISTANCE_FROM_BANK_FOR_POLICE_BESIEGE = Settings.GetValue<float>("SETTINGS", "MIN_DISTANCE_FROM_BANK_FOR_POLICE_BESIEGE", 100f);
            INITIAL_SPAWN_DELAY = Settings.GetValue<int>("SETTINGS", "INITIAL_SPAWN_DELAY", 120);
            LOADOUT_SET = Settings.GetValue<string>("SETTINGS", "LOADOUT_SET", "LOADOUT_SWAT_NO_LASER");
            HEAVY_UNIT_LOADOUT_OVERRIDE = Settings.GetValue<string>("SETTINGS", "HEAVY_UNIT_LOADOUT_OVERRIDE", "");
            SHIELD_MODEL = Settings.GetValue<string>("MODEL", "SHIELD_MODEL", "prop_ballistic_shield");
            // Updated code to look for modelnames instead of pedhashes, note this means that you need to update your config otherwise it will fallback to swat
            // civmale/civfemale pedtypes can cause in-fighting between them
            COP_MODEL_OVERRIDE = Settings.GetValue<string>("MODELS", "COP_MODEL_OVERRIDE", "s_m_y_swat_01");
            ARMY_MODEL_OVERRIDE = Settings.GetValue<string>("MODELS", "ARMY_MODEL_OVERRIDE", "s_m_y_marine_03");
            COP_COUNTRY_MODEL_OVERRIDE = Settings.GetValue<string>("MODELS", "COP_COUNTRY_MODEL_OVERRIDE", COP_MODEL_OVERRIDE);
            LC_MODEL_OVERRIDE = Settings.GetValue<string>("MODELS", "LC_MODEL_OVERRIDE", "ig_lcswat");
            PALETO_MODEL_OVERRIDE = Settings.GetValue<string>("MODELS", "PALETO_MODEL_OVERRIDE", "s_m_y_swat_01");
            PMC_MODEL_OVERRIDE = Settings.GetValue<string>("MODELS", "PMC_MODEL_OVERRIDE", "s_m_y_blackops_02");
            HEAVY_UNIT_MODEL_SWAT = Settings.GetValue<string>("MODELS", "HEAVY_UNIT_MODEL_SWAT", "hc_gunman");
            HEAVY_UNIT_MODEL_PMC = Settings.GetValue<string>("MODELS", "HEAVY_UNIT_MODEL_PMC", "U_M_M_Juggernaut_03");
            HEAVY_UNIT_MODEL_ARMY = Settings.GetValue<string>("MODELS", "HEAVY_UNIT_MODEL_ARMY", HEAVY_UNIT_MODEL_SWAT);

            ENABLE_WANTED_LEVEL_SPECIFIC_OVERRIDES = Settings.GetValue<bool>("MODELS", "ENABLE_WANTED_LEVEL_SPECIFIC_OVERRIDES", false);
            COP_WL3 = Settings.GetValue<string>("MODELS", "COP_WL3", COP_MODEL_OVERRIDE);
            COP_WL4 = Settings.GetValue<string>("MODELS", "COP_WL4", COP_MODEL_OVERRIDE);
            COP_WL5 = Settings.GetValue<string>("MODELS", "COP_WL5", COP_MODEL_OVERRIDE);

            FORCED_SNIPER_MODEL = Settings.GetValue<string>("MODELS", "FORCED_SNIPER_MODEL", "");

            SPAWN_EVERYWHERE = Settings.GetValue<bool>("SETTINGS", "SPAWN_EVERYWHERE", false);
            SHIELD_SPAWN_CHANCE_WL3 = Settings.GetValue<int>("SETTINGS", "SHIELD_SPAWN_CHANCE_WL3", 25);
            SHIELD_SPAWN_CHANCE_WL4 = Settings.GetValue<int>("SETTINGS", "SHIELD_SPAWN_CHANCE_WL4", 45);
            SHIELD_SPAWN_CHANCE_WL5 = Settings.GetValue<int>("SETTINGS", "SHIELD_SPAWN_CHANCE_WL5", 75);
            HEAVY_SPAWN_CHANCE_WL3 = Settings.GetValue<int>("SETTINGS", "HEAVY_SPAWN_CHANCE_WL3", 25);
            HEAVY_SPAWN_CHANCE_WL4 = Settings.GetValue<int>("SETTINGS", "HEAVY_SPAWN_CHANCE_WL4", 45);
            HEAVY_SPAWN_CHANCE_WL5 = Settings.GetValue<int>("SETTINGS", "HEAVY_SPAWN_CHANCE_WL5", 75);

            GRACE_PERIOD = Settings.GetValue<float>("SETTINGS", "GRACE_PERIOD", 60f);
            ASSAULT_DURATION = Settings.GetValue<float>("SETTINGS", "ASSAULT_DURATION", 150f);

            OLD_SPAWNING_SYSTEM = Settings.GetValue<bool>("SQUADS", "OLD_SPAWNING_SYSTEM", false);
            NUM_PER_SQUAD = Settings.GetValue<int>("SQUADS", "NUM_PER_SQUAD", 4); // 3 units will spawn
            SQUAD_FORMATION_TYPE = Settings.GetValue<FormationType>("SQUADS", "SQUAD_FORMATION_TYPE", FormationType.LineAroundLeader);

            // Allows user to customize how shields look, by default these are all set to prop_ballistic_shield
            USE_REGIONAL_SHIELDS = Settings.GetValue<bool>("SHIELDS", "USE_REGIONAL_SHIELDS", true);
            SWAT_SHIELD_MODEL = Settings.GetValue<string>("SHIELDS", "SWAT_SHIELD_MODEL", SHIELD_MODEL);
            ARMY_SHIELD_MODEL = Settings.GetValue<string>("SHIELDS", "ARMY_SHIELD_MODEL", SHIELD_MODEL);
            MWR_SHIELD_MODEL = Settings.GetValue<string>("SHIELDS", "MWR_SHIELD_MODEL", SHIELD_MODEL);
            LC_SHIELD_MODEL = Settings.GetValue<string>("SHIELDS", "LC_SHIELD_MODEL", SHIELD_MODEL);
            PALETO_SHIELD_MODEL = Settings.GetValue<string>("SHIELDS", "PALETO_SHIELD_MODEL", SHIELD_MODEL);

            NUM_CUSTOM_ZONES = Settings.GetValue<int>("ZONES", "NUM_CUSTOM_ZONES", 5);
            if (NUM_CUSTOM_ZONES <= 0)
            {
                NUM_CUSTOM_ZONES = 5;
            }

            DISABLE_RANDOM_PROPS = Settings.GetValue<bool>("SETTINGS", "DISABLE_RANDOM_PROPS", false);
            DISABLE_COMPONENTS = Settings.GetValue<bool>("SETTINGS", "DISABLE_COMPONENTS", false);
            DISABLE_RAMPAGES = Settings.GetValue<bool>("SETTINGS", "DISABLE_RAMPAGES", false);

            CHANCE_FOR_RESCUE_TEAM = Settings.GetValue<int>("SETTINGS", "CHANCE_FOR_RESCUE_TEAM", 25);

            ARMOR_LEVEL_1 = Settings.GetValue<int>("DIFFICULTY_SCALING", "ARMOR_LEVEL_1", 20);
            ARMOR_LEVEL_2 = Settings.GetValue<int>("DIFFICULTY_SCALING", "ARMOR_LEVEL_2", 25);
            ARMOR_LEVEL_3 = Settings.GetValue<int>("DIFFICULTY_SCALING", "ARMOR_LEVEL_3", 30);

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
            if (SPAWN_EVERYWHERE) return false;
            if (GetZoneType() == "NOOSE" || GetZoneType() == "MERRYWEATHER_JURISDICTION" || GetZoneType() == "LIBERTY_CITY")
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
			Vector3 pos = World.GetSafeCoordForPed(Game.Player.Character.Position + newPos, isForcedToSidewalks(), 24);

            return pos;
        }

        Model GetHeavyUnitModelByZone()
        {
            if (GetZoneType() == "ARMY")
            {
                return new Model(HEAVY_UNIT_MODEL_ARMY);
            }
            else if (GetZoneType() == "MERRYWEATHER_JURISDICTION")
            {
                return new Model(HEAVY_UNIT_MODEL_PMC);
            }
            return new Model(HEAVY_UNIT_MODEL_SWAT);
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
            else if (GetZoneType() == "PALETO")
            {
                return new Model(PALETO_MODEL_OVERRIDE);
            }
            else if (GetZoneType() == "NOOSE")
            {
                return new Model("s_m_y_swat_01");
            }
            else if (GetZoneType() == "MERRYWEATHER_JURISDICTION")
            {
                return new Model(PMC_MODEL_OVERRIDE);
            }
            else if (GetZoneType() == "LIBERTY_CITY")
            {
                return new Model(LC_MODEL_OVERRIDE);
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

        Model GetShieldModelByZone()
        {
            if (GetZoneType() == "ARMY")
            {
                return new Model(ARMY_SHIELD_MODEL);
            }
            else if (GetZoneType() == "MERRYWEATHER_JURISDICTION")
            {
                return new Model(MWR_SHIELD_MODEL);
            }
            else if (GetZoneType() == "NOOSE")
            {
                return new Model(SWAT_SHIELD_MODEL);
            }
            else if (GetZoneType() == "LIBERTY_CITY")
            {
                return new Model(LC_SHIELD_MODEL);
            }
            else if (GetZoneType() == "PALETO")
            {
                return new Model(PALETO_SHIELD_MODEL);
            }

            return new Model(SWAT_SHIELD_MODEL);
        }

        bool IsLiberty()
        {
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZBOO11"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZBOO12"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZBOO13"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZBOO14"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZBOO15"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZBOO16"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZBOO17"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZBOO18"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZBOO19"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZBOO120"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZBOO121"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZBOO123"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZBRI1"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZBRI2"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZBRI4"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZBRI5"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZBRI6"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZBRI7"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZBRI8"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZPENN1"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZPENN2"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZPENN3"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZPENN4"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZPENN5"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZLEAP"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zact"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zact1"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zact2"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZAIRPT1"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZAIRPT2"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZAIRPT3"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zald1"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zald2"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zald3"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zald4"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZBEG1"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZBEG2"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZBECCT1"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZBECCT2"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZBECCT3"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZBECCT4"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZBECCT5"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zberc"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zberc1"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZBOAB1"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZBOAB2"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZBOAB3"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZBOAB4"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZBOAB5"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZBOAB6"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZBOULE1"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZBOULE2"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZBOULE3"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZBOULE4"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZCERV1"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZCERV2"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZCERV3"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZCERV4"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zcgar1"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zcgar2"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zcgar3"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zcgar4"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zcgci1"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zcgci2"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zcgci3"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zcgci4"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZCHASE1"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZCHASE2"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zchin"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zchisl1"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zchisl2"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZCity1"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZCity2"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zcois1"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zcois3"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zcois4"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zcois5"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zcois8"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZDOWNT"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zeast"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZEHOK1"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZEHOK2"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZEHOK3"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZEHOK4"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZEHOK5"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZEHOK6"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZEHOK7"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZEHOK8"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZEHOK9"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZEHOK10"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zehol1"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zehol2"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZESTCT1"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZESTCT2"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZESTCT3"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZESTCT4"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZESTCT5"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZESTCT6"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZESTCT7"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZESTCT8"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZESTCT9"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zexc1"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zexc2"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zexc3"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zexc4"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZFIEPR1"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZFIEPR2"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZFIEPR3"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZFIEPR4"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZFIEPR5"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZFIEPR6"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zfisn"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zfisn1"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zfisn2"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zfisn3"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zfisn4"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zfisn5"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zfort"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZFRIS1"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZFRIS2"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZHap1"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZHap2"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZHap3"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZHap1"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zhat"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZHOVEB1"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZHOVEB2"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZHOVEB3"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZINDUS1"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZINDUS2"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZINDUS3"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZINDUS3"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZINDUS4"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZINDUS5"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZINDUS6"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zital"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zlanc1"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zlanc2"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zlance"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZLBAY1"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZLBAY2"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZLBAY3"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZLBAY4"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZLBAY5"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zleft1"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zleft2"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zlowe1"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zlowe2"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zmdw1"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zmdw2"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zmdw3"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zmeat"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZMHILLS"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zmide"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zmidpa"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZMPARK1"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZMPARK2"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZMPARK3"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZMPARK4"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Znhol1"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Znhol2"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Znorm"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Znort1"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Znort2"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Znort3"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Znort4"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Znort5"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZNRDNS1"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZNRDNS2"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZNRDNS3"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZNRDNS4"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZNRDNS5"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZNRDNS6"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZNRDNS7"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZOUTLO"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zport"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zport1"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zpres"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zpurg1"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zpurg2"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zpurg3"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZRHIL1"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZRHIL2"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZRHIL3"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZRHIL4"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZRHIL5"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZRHIL6"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZRHIL7"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZRHIL8"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZRHIL9"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZRHIL10"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZRHIL11"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZRHIL12"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZRHIL13"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZRHIL14"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZRHIL15"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZRHIL16"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZSHTLER"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZSLOPES"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZSOHAN1"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZSOHAN2"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zstar"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZSTEI1"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZSTEI2"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZSTEI3"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZSTEI4"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZSTEI5"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZSTEI6"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZSTEI7"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZSTEI8"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZSTEI9"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZSTEI10"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZSTEI11"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zsuff1"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zsuff2"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Ztri"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Ztudo1"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Ztudo2"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zvarh"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zwest1"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zwest2"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zwestm"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZWILIS1"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZAIRU0"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZAIRU1"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZAIRU2"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZAIRU3"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZAIRU4"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZAIRU5"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZAIRU6"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZAIRU7"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZAIRU8"))
                return true;
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZAIRU9"))
                return true;
            return false;
        }

        string GetZoneType()
        {
            string customZoneIndex = "CUSTOM_ZONE_" + NUM_CUSTOM_ZONES.ToString();
            if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ArmyB") || Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Zancudo"))
            {
                return "ARMY";
            }
            else if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Desrt") || Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Alamo") || Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Lago") || Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Slab") || Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Sandy"))
            {
                return "DESERT";
            }
            else if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "Paleto"))
            {
                return "PALETO";
            }
            else if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "IsHeistZone"))
            {
                return "MERRYWEATHER_JURISDICTION"; //CAYO_PERICO
            }
            else if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "NOOSE"))
            {
                return "NOOSE";
            }
            else if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "GOLF"))
            {
                return "NOOSE";
            }
            else if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ELYSIAN") || Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "TERMINA"))
            {
                return "MERRYWEATHER_JURISDICTION";
            }
            else if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "JAIL"))
            {
                return "NOOSE";
            }
            else if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "OCEANA"))
            {
                return "MERRYWEATHER_JURISDICTION";
            }
            else if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, "ZP_ORT"))
            {
                return "MERRYWEATHER_JURISDICTION";
            }
            else if (Function.Call<bool>(Hash.IS_ENTITY_IN_ZONE, Game.Player.Character, customZoneIndex))
            {
                return "MERRYWEATHER_JURISDICTION";
            }
            else if (IsLiberty())
            {
                return "LIBERTY_CITY";
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
                    m_fTimeUntilNextWave = Game.GameTime + GRACE_PERIOD;
                    underAssault = true;
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
                if (OLD_SPAWNING_SYSTEM)
                {
                    if (random.Next(1, 100) < GetShieldSpawnChance() && shields.Count < MAX_SHIELDS)
                    {
                        SpawnShield(pos);
                    }
                    else if (random.Next(1, 100) < GetHeavySpawnChance())
                    {
                        SpawnUnit(pos, 0, false, true);
                    }
                    else
                    {
                        SpawnUnit(pos);
                    }
                }
                else
                {
                    // Squads are handled in groups
                    // Each having a leader, whom they will follow depending on the SQUAD_FORMATION_TYPE
                    // Sometimes the leader may spawn with a shield
                    if (random.Next(1, 100) < GetHeavySpawnChance() && heavies.Count < MAX_HEAVIES)
                    {
                        if (Game.GameTime % TIME_BETWEEN_SPAWNS == 0)
                        {
                            SpawnUnit(pos, 0, false, true);
                        }
                    }
                    else
                    {
                        if (Game.GameTime % TIME_BETWEEN_SPAWNS == 0)
                        {
                            int group = Function.Call<int>(Hash.CREATE_GROUP);
                            bool isShield = random.Next(1, 100) < GetShieldSpawnChance() && shields.Count < MAX_SHIELDS;
                            bool hasSpawnedLeader = false;
                            // Spawn the leader
                            // If unsuccessful, early out
                            if (isShield)
                            {
                                hasSpawnedLeader = SpawnShield(pos, group, true);
                                Function.Call(Hash.SET_GROUP_FORMATION, group, FormationType.LineAroundLeader);
                            }
                            else
                            {
                                hasSpawnedLeader = SpawnUnit(pos, group, true);
                                Function.Call(Hash.SET_GROUP_FORMATION, group, SQUAD_FORMATION_TYPE);
                            }
                            if (!hasSpawnedLeader) return;
                            // Spawn the rest of the squad
                            for (int i = 0; i <= NUM_PER_SQUAD; i++)
                            {
                                SpawnUnit(pos, group);
                            }
                        }
                    }
                }
            }
        }

        void UpdateGroups()
        {
            if (Game.Player.WantedLevel >= MAX_WANTED_LEVEL && canSpawn() && canSpawnPeds)
            {
                bool IS_SEEN_BY_COPS = Function.Call<bool>(Hash.IS_WANTED_AND_HAS_BEEN_SEEN_BY_COPS, Game.Player);
                if (Game.Player.Character.IsOnFoot && IS_SEEN_BY_COPS)
                {
                    if (OLD_SPAWNING_SYSTEM)
                    {
                        if (Game.GameTime % TIME_BETWEEN_SPAWNS == 0)
                        {
                            SpawnGroup();
                        }
                    }
                    else
                    {
                        if (underAssault)
                        {
                            if (Game.GameTime - m_fTimeUntilNextWave > ASSAULT_DURATION)
                            {
                                m_fTimeUntilNextWave = Game.GameTime + GRACE_PERIOD;
                                underAssault = true;
                                return;
                            }

                            SpawnGroup();
                        }
                        else
                        {
                            if (Game.GameTime - m_fTimeUntilNextWave > GRACE_PERIOD)
                            {
                                m_fTimeUntilNextWave = Game.GameTime + ASSAULT_DURATION;
                                underAssault = false;
                                return;
                            }

                            Vector3 initialPos = Vector3.Zero;

                            // Check how many "fleeing" civs there are, assumption cowering peds are treated the same
                            int countHostages()
                            {
                                int count = 0;
                                var near_peds = World.GetNearbyPeds(Game.Player.Character, 35f);
                                foreach (Ped p in near_peds)
                                {
                                    if (p != Game.Player.Character && !p.IsPlayer && p.IsHuman && p.IsFleeing)
                                    {
                                        if (initialPos == Vector3.Zero)
                                        {
                                            initialPos = p.Position;
                                        }    
                                        count++;
                                    }
                                }
                                return count;
                            }

                            // Spawn a "rescue" team
                            void SpawnRescueTeam(Vector3 pos)
                            {
                                var model = GetModelByZone();
                                if (!model.IsLoaded)
                                {
                                    model.Request(5);
                                }

                                if (model.IsInCdImage && model.IsValid)
                                {
                                    var ped = World.CreatePed(model, pos);
                                    ped.KeepTaskWhenMarkedAsNoLongerNeeded = true;
                                    ped.Task.GoStraightTo(initialPos);

                                    var hash = Function.Call<int>(Hash.GET_HASH_KEY, LOADOUT_SET);
                                    Function.Call(Hash.GIVE_LOADOUT_TO_PED, ped, hash);

                                    ped.RelationshipGroup = RelationshipGroupHash.Cop;

                                    AddHelmet(ped);
                                    SetComponent(ped);
                                    // Adjust Combat Flags (See: https://docs.fivem.net/natives/?_0x9F7794730795E019)
                                    Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 60, false); // prevent smoke grenade throwing
                                    Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 22, true); // allow dragging allies
                                    Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 21, false); // prevent chasing target onfoot
                                    Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 41, false); // prevent commandering vehicles
                                    Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 42, false); // prevent flanking
                                    Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 50, false); // prevent charging
                                    Function.Call(Hash.SET_PED_COMBAT_MOVEMENT, ped, 1); // CM_Defensive

                                    groups.Add(ped);
                                    rescue_hrt.Add(ped.Handle, ped);
                                    model.MarkAsNoLongerNeeded();
                                }
                            }

                            if (countHostages() >= 4)
                            {
                                if (rescue_hrt.Count <= 0)
                                {
                                    return;
                                }
                                else
                                {
                                    for (int i = rescue_hrt.Count - 1; i > -1; i--)
                                    {
                                        var ped = rescue_hrt[i];
                                        if (ped != null && ped.Exists())
                                        {
                                            var near_peds = World.GetNearbyPeds(ped, 10f);
                                            foreach (Ped civ in near_peds)
                                            {
                                                if (civ != ped && !civ.IsPersistent && !civ.IsPlayer && civ.IsHuman)
                                                {
                                                    civ.Task.FleeFrom(Game.Player.Character);
                                                }
                                            }
                                        }
                                    }
                                }
                                // Every 25 seconds with a 25% chance, a spawn attempt is made
                                if (Game.GameTime % 25000 == 0 && random.Next(1, 100) < CHANCE_FOR_RESCUE_TEAM)
                                {
                                    var pos = FindAvailableSpawnPoint();
                                    if (pos != Vector3.Zero)
                                    {
                                        for (int i = 0; i <= 4; i++)
                                        {
                                            SpawnRescueTeam(pos);
                                        }
                                    }
                                }
                            }
                        }
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
                            ped.KeepTaskWhenMarkedAsNoLongerNeeded = false;
                            ped.CanRagdoll = true;
                            if (shields.ContainsKey(ped.Handle) && shields[ped.Handle] != null && shields[ped.Handle].Exists())
                            {
                                DestroyShield(ped, shields[ped.Handle]);
                                shields.Remove(ped.Handle);
                            }
                            else if (rescue_hrt.ContainsKey(ped.Handle) && rescue_hrt[ped.Handle] != null && rescue_hrt[ped.Handle].Exists())
                            {
                                rescue_hrt.Remove(ped.Handle);
                            }
                            else if (heavies.ContainsKey(ped.Handle) && heavies[ped.Handle] != null && heavies[ped.Handle].Exists())
                            {
                                heavies.Remove(ped.Handle);
                            }
                            if (ped.AttachedBlip != null && ped.AttachedBlip.Exists())
                            {
                                ped.AttachedBlip.Delete();
                            }
                            ped.MarkAsNoLongerNeeded();
                            groups.RemoveAt(i);
                        }
                        else
                        {
                            if (ped.IsDead || Game.Player.WantedLevel <= 0 || Game.Player.IsDead || World.GetDistance(ped.Position, Game.Player.Character.Position) > MIN_POLICE_DESPAWN_RANGE)
                            {
                                ped.KeepTaskWhenMarkedAsNoLongerNeeded = false;
                                ped.CanRagdoll = true;
                                if (shields.ContainsKey(ped.Handle) && shields[ped.Handle] != null && shields[ped.Handle].Exists())
                                {
                                    if (ped.IsDead)
                                    {
                                        DetachShield(ped, shields[ped.Handle]);
                                    }
                                    else
                                    {
                                        DestroyShield(ped, shields[ped.Handle]);
                                    }
                                    shields.Remove(ped.Handle);
                                }
                                else if (rescue_hrt.ContainsKey(ped.Handle) && rescue_hrt[ped.Handle] != null && rescue_hrt[ped.Handle].Exists())
                                {
                                    rescue_hrt.Remove(ped.Handle);
                                }
                                else if (heavies.ContainsKey(ped.Handle) && heavies[ped.Handle] != null && heavies[ped.Handle].Exists())
                                {
                                    heavies.Remove(ped.Handle);
                                }
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

        // This is defaulted to OFF, but i am keeping this anyway
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
                    cop1.Weapons.Current.InfiniteAmmo = true;
                    cop1.KeepTaskWhenMarkedAsNoLongerNeeded = true;
                    cop1.Task.Combat(Game.Player.Character);
                    Function.Call(Hash.SET_PED_COMBAT_MOVEMENT, cop1, 0);
                    Function.Call(Hash.SET_PED_COMBAT_RANGE, cop1, 3);
                    Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, cop1, 46, true);
                    cop1.MarkAsNoLongerNeeded();
                }

                var sniper1 = World.CreatePed(PedHash.Swat01SMY, new Vector3(176.758759f, 198.06012f, 104.948158f), heading: -1.832596f);
                if (sniper1 != null && sniper1.Exists())
                {
                    sniper1.KeepTaskWhenMarkedAsNoLongerNeeded = true;
                    sniper1.Weapons.Give(WeaponHash.SniperRifle, 9999, true, true);
                    sniper1.Weapons.Current.InfiniteAmmo = true;
                    sniper1.Task.Combat(Game.Player.Character);
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

        float GetRampageTimeLimit()
        {
            if (DIFFICULTY == 0 || DIFFICULTY == 1)
            {
                return 300000f; // 5 minutes
            }
            else if (DIFFICULTY == 2)
            {
                return 600000f; // 10 minutes
            }
            else if (DIFFICULTY == 3)
            {
                return 900000f; // 15 minutes
            }

            // Default fallback
            return 600000f; // 10 minutes
        }

        string GetTimeLimitText(float timeLimit)
        {
            if (timeLimit == 300000f) // 5 min
                return "5 minutes";
            else if (timeLimit == 600000f) // 10 min
                return "10 minutes";
            else if (timeLimit == 900000f) // 15 min
                return "15 minutes";

            // Default fallback
            return "10 minutes";
        }

        bool IsTrevor()
        {
            return Game.Player.Character.Model == "player_two";
        }

        void OnTick(object sender, EventArgs e)
        {
            UpdateState();
            UpdateGroups();
            UpdateExistingMembers();
            SpawnPoliceBesiege();

            if (!m_bIsInitialFirstTime)
            {
                m_bIsInitialFirstTime = true;
                m_fTimeUntilNextWave = Game.GameTime + GRACE_PERIOD;

                if (!DISABLE_RAMPAGES)
                {
                    Blip blipPH = World.CreateBlip(pillboxHillLocation);
                    blipPH.Alpha = 255;
                    blipPH.Sprite = BlipSprite.Rampage;
                    blipPH.Color = BlipColor.Trevor;
                    blipPH.Name = "Rampage"; // Rampage: Pillbox Hill

                    map_markers.Add(blipPH);

                    Blip blipMissionRow = World.CreateBlip(missionRowLocation);
                    blipMissionRow.Alpha = 255;
                    blipMissionRow.Sprite = BlipSprite.Rampage;
                    blipMissionRow.Color = BlipColor.Trevor;
                    blipMissionRow.Name = "Rampage"; // Rampage: Mission Row

                    map_markers.Add(blipMissionRow);

                    Blip blipBPLocation = World.CreateBlip(blitzPlayLoc);
                    blipBPLocation.Alpha = 255;
                    blipBPLocation.Sprite = BlipSprite.Rampage;
                    blipBPLocation.Color = BlipColor.Trevor;
                    blipBPLocation.Name = "Rampage"; // Rampage: Blitz Play

                    map_markers.Add(blipBPLocation);

                    Blip blipMWDocks = World.CreateBlip(merryweatherDocks);
                    blipMWDocks.Alpha = 255;
                    blipMWDocks.Sprite = BlipSprite.Rampage;
                    blipMWDocks.Color = BlipColor.Trevor;
                    blipMWDocks.Name = "Rampage"; // Rampage: Merryweather Docks

                    map_markers.Add(blipMWDocks);
                }
            }

            if (!DISABLE_RAMPAGES)
            {
                if (!m_bIsRampageActive)
                {
                    if (World.GetDistance(Game.Player.Character.Position, pillboxHillLocation) < 6 && IsTrevor())
                    {
                        GTA.UI.Screen.ShowHelpText("Press ~INPUT_CONTEXT~ to start rampage", 25000, true, true);
                        if (Game.IsControlJustPressed(Control.Context))
                        {
                            float timeLimit = GetRampageTimeLimit();
                            m_bIsRampageActive = true;
                            m_fTimeUntilRampageOver = Game.GameTime + timeLimit;
                            Game.Player.WantedLevel = MAX_WANTED_LEVEL;
                            Game.Player.WantedCenterPosition = pillboxHillLocation;
                            GTA.UI.Screen.ShowHelpText("Survive for " + GetTimeLimitText(timeLimit), 25000);
                        }
                    }
                    else if (World.GetDistance(Game.Player.Character.Position, missionRowLocation) < 6 && IsTrevor())
                    {
                        GTA.UI.Screen.ShowHelpText("Press ~INPUT_CONTEXT~ to start rampage", 25000, true, true);
                        if (Game.IsControlJustPressed(Control.Context))
                        {
                            float timeLimit = GetRampageTimeLimit();
                            m_bIsRampageActive = true;
                            m_fTimeUntilRampageOver = Game.GameTime + timeLimit;
                            Game.Player.WantedLevel = MAX_WANTED_LEVEL;
                            Game.Player.WantedCenterPosition = missionRowLocation;
                            GTA.UI.Screen.ShowHelpText("Survive for " + GetTimeLimitText(timeLimit), 25000);
                        }
                    }
                    else if (World.GetDistance(Game.Player.Character.Position, blitzPlayLoc) < 6 && IsTrevor())
                    {
                        GTA.UI.Screen.ShowHelpText("Press ~INPUT_CONTEXT~ to start rampage", 25000, true, true);
                        if (Game.IsControlJustPressed(Control.Context))
                        {
                            float timeLimit = GetRampageTimeLimit();
                            m_bIsRampageActive = true;
                            m_fTimeUntilRampageOver = Game.GameTime + timeLimit;
                            Game.Player.WantedLevel = MAX_WANTED_LEVEL;
                            Game.Player.WantedCenterPosition = blitzPlayLoc;
                            GTA.UI.Screen.ShowHelpText("Survive for " + GetTimeLimitText(timeLimit), 25000);
                        }
                    }
                    else if (World.GetDistance(Game.Player.Character.Position, merryweatherDocks) < 6 && IsTrevor())
                    {
                        GTA.UI.Screen.ShowHelpText("Press ~INPUT_CONTEXT~ to start rampage", 25000, true, true);
                        if (Game.IsControlJustPressed(Control.Context))
                        {
                            float timeLimit = GetRampageTimeLimit();
                            m_bIsRampageActive = true;
                            m_fTimeUntilRampageOver = Game.GameTime + timeLimit;
                            Game.Player.WantedLevel = MAX_WANTED_LEVEL;
                            Game.Player.WantedCenterPosition = merryweatherDocks;
                            GTA.UI.Screen.ShowHelpText("Survive for " + GetTimeLimitText(timeLimit), 25000);
                        }
                    }
                }
                else
                {
                    if (Game.GameTime - m_fTimeUntilRampageOver > GetRampageTimeLimit())
                    {
                        m_bIsRampageActive = false;
                        m_fTimeUntilRampageOver = Game.GameTime;
                        GTA.UI.Screen.ShowHelpText("Enemies defeated! Reward: 250K", 25000);
                        Game.Player.WantedLevel = 0;
                        Game.Player.Money += 250;

                        float x = Game.Player.Character.Position.X;
                        float y = Game.Player.Character.Position.Y;
                        float z = Game.Player.Character.Position.Z;
                        Function.Call(Hash.CLEAR_AREA_OF_COPS, x, y, z, 100000f, true);
                    }
                }
            }
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

            if (map_markers.Count > 0)
            {
                for (int i = map_markers.Count - 1; i > -1; i--)
                {
                    var blip = map_markers[i];
                    if (blip != null && blip.Exists())
                    {
                        blip.Delete();
                        map_markers.RemoveAt(i);
                    }
                }
            }
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