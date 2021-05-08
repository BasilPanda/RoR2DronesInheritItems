using System;
using System.Linq;
using System.Collections.Generic;
using RoR2;
using RoR2.CharacterAI;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.ObjectModel;

namespace Basil_ror2
{
    public static class Hooks
    {
        // Bodyprefabs to check
        public static String[] bodyprefabNames = new String[]
        {
            "SquidTurretBody",
            "Turret1Body",
            "Drone1Body",
            "Drone2Body",
            "MegaDroneBody",
            "MissileDroneBody",
            "FlameDroneBody",
            "EquipmentDroneBody",
            "EmergencyDroneBody"
        };

        // Master prefabs to check in Open Summon Return. Squid turrets do not get spawned in this method.
        public static Dictionary<string, bool> masterPrefabNamesSummonReturn = new Dictionary<string, bool>()
        {
            { "Turret1Master(Clone)", DII.MinigunTurretsInherit.Value },
            { "Drone1Master(Clone)", DII.GunnerDronesInherit.Value },
            { "Drone2Master(Clone)", DII.HealDronesInherit.Value },
            { "DroneMissileMaster(Clone)", DII.ProtoDronesInherit.Value},
            { "MegaDroneMaster(Clone)", DII.MissileDronesInherit.Value },
            { "FlameDroneMaster(Clone)", DII.FlameDronesInherit.Value },
            { "EquipmentDroneMaster(Clone)", DII.EquipDronesInherit.Value },
            { "EmergencyDroneMaster(Clone)", DII.EmergencyDronesInherit.Value },
            { "SquidTurretMaster(Clone)", DII.SquidTurretsInherit.Value },
            { "BeetleGuardAllyMaster(Clone)", DII.QueenGuardInherit.Value },
            { "DroneBackupMaster(Clone)", DII.BackupDronesInherit.Value },
            { "TitanGoldAllyMaster(Clone)", DII.GoldTitanInherit.Value },
            { "RoboBallRedBuddyMaster(Clone)",  DII.SolusInherit.Value},
            { "RoboBallGreenBuddyMaster(Clone)",  DII.SolusInherit.Value},
        };

        // Config Values for updating stage
        public static Dictionary<string, bool> bodyprefabsDict = new Dictionary<string, bool>()
        {
            {"SquidTurretBody", DII.SquidTurretsInherit.Value },
            {"Turret1Body", DII.MinigunTurretsInherit.Value },
            {"Drone1Body", DII.GunnerDronesInherit.Value},
            {"Drone2Body", DII.HealDronesInherit.Value},
            {"MegaDroneBody", DII.ProtoDronesInherit.Value},
            {"MissileDroneBody", DII.MissileDronesInherit.Value},
            {"FlameDroneBody", DII.FlameDronesInherit.Value},
            {"EquipmentDroneBody", DII.EquipDronesInherit.Value},
            {"EmergencyDroneBody", DII.EmergencyDronesInherit.Value},
        };

        // Master names to check
        public static String[] masterNames = new String[]
        {
            "Drone1Master",
            "Drone2Master",
            "DroneBackupMaster",
            "DroneMissileMaster",
            "Turret1Master",
            "MegaDroneMaster",
            "FlameDroneMaster",
            "EquipmentDroneMaster",
            "TitanGoldMaster",
            "EmergencyDroneMaster",
            "BeetleGuardAllyMaster",
            "DroneBackupMaster"
        };

        // Update inventories after every stage 
        public static void updateAfterStage()
        {
            if (DII.UpdateInventory.Value)
            {
                On.RoR2.Run.AdvanceStage += (orig, self, nextScene) =>
                {
                    var masters = CharacterMaster.readOnlyInstancesList;
                    List<CharacterMaster> masterList = masters.Cast<CharacterMaster>().ToList();
                    foreach (CharacterMaster cm in masterList)
                    {
                        if (bodyprefabNames.Contains(cm.bodyPrefab.name))
                        {
                            if (cm.bodyPrefab.name == "SquidTurretBody")
                            {
                                continue;
                            }
                            if(cm.bodyPrefab.name == "MegaDroneBody" && SceneManager.GetActiveScene().name == "moon")
                            {
                                continue;
                            }
                            if (bodyprefabsDict[cm.bodyPrefab.name])
                            {
                                CharacterMaster owner = cm.gameObject.GetComponent<AIOwnership>().ownerMaster;
                                DII.updateInventory(cm, owner);
                                if(cm.bodyPrefab.name == "EquipmentDroneBody")
                                {
                                    cm.inventory.GiveItem((ItemIndex)121, 15);
                                }
                            }
                        }
                    }
                    orig(self, nextScene);
                };
            }
        }

        // Ghost inheritance
        public static void spookyGhosts()
        {
            if (DII.GhostInherit.Value)
            {
                On.RoR2.Util.TryToCreateGhost += (orig, targetBody, ownerBody, duration) =>
                {
                    CharacterBody characterBody = orig(targetBody, ownerBody, duration);
                    if(characterBody == null)
                    {
                        return null;
                    }
                    CharacterMaster cm = characterBody.master;
                    AIOwnership component2 = cm.gameObject.GetComponent<AIOwnership>();
                    CharacterMaster master = ownerBody.master;
                    DII.checkConfig(cm, master);
                    
                    CustomBlacklist.customItem(cm, DII.CBItemGhosts.Value);
                    CustomBlacklist.customItemCap(cm, DII.CBItemCapGhosts.Value);
                    CustomBlacklist.customEquip(cm, DII.CBEquipGhosts.Value);
                    cm.inventory.GiveItem(RoR2Content.Items.Ghost, 1);
                    cm.inventory.ResetItem(RoR2Content.Items.HealthDecay);
                    cm.inventory.GiveItem(RoR2Content.Items.HealthDecay, duration);
                    return characterBody;
                };
            }
        }

        // Everything but Ghost hook
        public static void masterInherit(MasterSummon.MasterSummonReport masterSummon)
        {
            CharacterMaster npc = masterSummon.summonMasterInstance;
            CharacterMaster player = masterSummon.leaderMasterInstance;
            if(npc == null)
            {
                return;
            }
            if (npc.teamIndex != TeamIndex.Player)
            {
                return;
            }
            // Debug.Log("Master Inherit: " + npc.name);

            // Titan handler.
            if (player == null)
            {
                if (npc.name == "TitanGoldAllyMaster(Clone)")
                {
                    ReadOnlyCollection<TeamComponent> teamMembers = TeamComponent.GetTeamMembers(TeamIndex.Player);
                    int highestSeedCount = 0;
                    int playerIndex = 0;
                    for (int i = 0; i < teamMembers.Count; i++)
                    {
                        if (Util.LookUpBodyNetworkUser(teamMembers[i].gameObject))
                        {
                            CharacterBody component = teamMembers[i].GetComponent<CharacterBody>();
                            int count = component.inventory.GetItemCount(RoR2Content.Items.TitanGoldDuringTP);
                            if (highestSeedCount < count)
                            {
                                highestSeedCount = count;
                                playerIndex = i;
                            }
                        }
                    }
                    player = teamMembers[playerIndex].GetComponent<CharacterBody>().master;
                }
                else
                {
                    return;
                }
            }
            masterKeyHandler(npc, player);
        }

        // Event attaching
        public static void baseMod()
        {
            MasterSummon.onServerMasterSummonGlobal += masterInherit;
        }

        // Fixes Dio reinheritance
        public static void fixBackupDio()
        {
            if (DII.FixBackupDio.Value)
            {
                On.RoR2.CharacterMaster.RespawnExtraLife += (orig, self) =>
                {
                    orig(self);

                    if (self.bodyPrefab.name.ToString() == "BackupDroneBody")
                    {
                        self.gameObject.AddComponent<MasterSuicideOnTimer>().lifeTimer = DII.ConfigToFloat(DII.BackupDeathTimer.Value) + UnityEngine.Random.Range(0f, 3f);
                    }
                };
            }
        }

        // Check keys and add to inventory
        public static void masterKeyHandler(CharacterMaster npc, CharacterMaster player)
        {
            //Debug.Log(npc.name);
            if (masterPrefabNamesSummonReturn.Keys.Contains(npc.name) && npc.teamIndex == TeamIndex.Player)
            {
                if (masterPrefabNamesSummonReturn[npc.name])
                {
                    DII.checkConfig(npc, player);
                    if (npc.name == "Drone2Master" || npc.name == "Emer")
                    {
                        npc.inventory.ResetItem(RoR2Content.Items.LunarPrimaryReplacement);
                    }
                    if (npc.name == "SquidTurretMaster(Clone)")
                    {
                        npc.inventory.GiveItem(RoR2Content.Items.HealthDecay, (int)Math.Ceiling(DII.ConfigToFloat(DII.SquidHealthDecay.Value)));
                    }
                }
            }
        }

        
    }
}
