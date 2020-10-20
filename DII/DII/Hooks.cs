using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using RoR2;
using UnityEngine;
using RoR2.CharacterAI;
using UnityEngine.Networking;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine.SceneManagement;

namespace Basil_ror2
{
    public static class Hooks
    {
        private static System.Random rand = new System.Random();

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
            { "Turret1Master", DII.MinigunTurretsInherit.Value },
            { "Drone1Master", DII.GunnerDronesInherit.Value },
            { "Drone2Master", DII.HealDronesInherit.Value },
            { "DroneMissileMaster", DII.ProtoDronesInherit.Value},
            { "MegaDroneMaster", DII.MissileDronesInherit.Value },
            { "FlameDroneMaster", DII.FlameDronesInherit.Value },
            { "EquipmentDroneMaster", DII.EquipDronesInherit.Value },
            { "EmergencyDroneMaster", DII.EmergencyDronesInherit.Value },
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
                    cm.inventory.GiveItem(ItemIndex.Ghost, 1);
                    cm.inventory.ResetItem(ItemIndex.HealthDecay);
                    cm.inventory.GiveItem(ItemIndex.HealthDecay, duration);
                    return characterBody;
                };
            }
        }

        // Queen Guard inheritance
        public static void queensGuard()
        {
            if (DII.QueenGuardInherit.Value)
            {
                IL.RoR2.CharacterBody.UpdateBeetleGuardAllies += il =>
                {
                    ILCursor c = new ILCursor(il);
                    c.GotoNext(
                        x => x.MatchLdloc(4),
                        x => x.MatchCallOrCallvirt<CharacterMaster>("get_inventory"),
                        x => x.MatchLdcI4(47),
                        x => x.MatchLdcI4(10),
                        x => x.MatchCallOrCallvirt<Inventory>("GiveItem"),
                        x => x.MatchLdloc(4)
                        );
                    c.Index += 5;
                    c.Emit(OpCodes.Ldarg_0);
                    c.Emit(OpCodes.Call, typeof(CharacterBody).GetMethod("get_master"));
                    c.Emit(OpCodes.Ldloc, 4);
                    c.EmitDelegate <Action<CharacterMaster, CharacterMaster>>((player, beetle) =>
                      {
                          if (beetle && player)
                          {
                              DII.checkConfig(beetle, player);
                          }
                      });
                    //Debug.Log(c.ToString());
                };
            }
                /*
            if (DII.QueenGuardInherit.Value)
            {
                On.RoR2.DirectorCore.TrySpawnObject += (orig, self, directorSpawnRequest) =>
                {
                    GameObject gameObject = orig(self, directorSpawnRequest);
                    CharacterMaster cm = gameObject.GetComponent<CharacterMaster>();
                    if (gameObject && cm && cm.bodyPrefab.name == "BeetleGuardAllyBody")
                    {
                        if(cm.GetComponent<AIOwnership>().ownerMaster)
                        {
                            DII.checkConfig(cm, cm.GetComponent<AIOwnership>().ownerMaster);
                        }
                    }
                    return gameObject;
                };
            }*/
        }
            
        // Backup Drone inheritance 
        public static void backupDrones()
        {
            if (DII.BackupDronesInherit.Value)
            {
                On.RoR2.EquipmentSlot.SummonMaster += (orig, self, masterObjectPrefab, position, rotation) =>
                {
                    CharacterMaster characterMaster = orig(self, masterObjectPrefab, position, rotation);
                    
                    if (DII.BackupDronesInherit.Value && masterObjectPrefab.name == "DroneBackupMaster" && characterMaster != null)
                    {
                        DII.checkConfig(characterMaster, characterMaster.gameObject.GetComponent<AIOwnership>().ownerMaster);
                        characterMaster.inventory.ResetItem(ItemIndex.AutoCastEquipment);
                    }
                    return characterMaster;
                };
            }
            
        }
        
        // Aurelionite
        public static void titanGold()
        {
            On.RoR2.TeleporterInteraction.ChargingState.TrySpawnTitanGoldServer += (orig, self) =>
            {

                orig(self);
                CharacterMaster titan = self.GetFieldValue<CharacterMaster>("titanGoldBossMaster");
                CharacterMaster cm = null;
                ReadOnlyCollection<TeamComponent> teamMembers = TeamComponent.GetTeamMembers(TeamIndex.Player);
                for (int i = 0; i < teamMembers.Count; i++)
                {
                    if (Util.LookUpBodyNetworkUser(teamMembers[i].gameObject))
                    {
                        CharacterBody component = teamMembers[i].GetComponent<CharacterBody>();
                        if (component && component.inventory)
                        {
                            cm = component.master;
                        }
                    }
                }
                if (DII.GoldTitanInherit.Value && cm != null && titan != null)
                {
                    DII.checkConfig(titan, cm);
                }
            };

        }
        
        // Squid Turrets 
        public static void squidInherit(SpawnCard.SpawnResult spawnResult)
        {
            CharacterMaster squidy = spawnResult.spawnedInstance ? spawnResult.spawnedInstance.GetComponent<CharacterMaster>() : null;
            if (!squidy)
            {
                return;
            }
            //Debug.Log(squidy.name);
            if (squidy.name == "SquidTurretMaster(Clone)")
            {
                //CharacterMaster player = PlayerCharacterMasterController.instances[rand.Next(0, Run.instance.livingPlayerCount)].master;
                CharacterMaster player = spawnResult.spawnRequest.summonerBodyObject.GetComponent<CharacterBody>().master;
                DII.checkConfig(squidy, player);
                if(DII.FixSquid.Value)
                {
                    squidy.inventory.GiveItem(ItemIndex.HealthDecay,(int)Math.Ceiling(DII.ConfigToFloat(DII.SquidHealthDecay.Value)));
                }
            }
            
        }

        // Drones and Turrets
        public static void baseMod()
        {
            if (DII.SquidTurretsInherit.Value)
                SpawnCard.onSpawnedServerGlobal += squidInherit;

            On.RoR2.SummonMasterBehavior.OpenSummonReturnMaster += (orig, self, activator) =>
            {
                CharacterMaster characterMaster = orig(self, activator);
                
                Inventory inventory = characterMaster.inventory;
                CharacterBody cm = activator.GetComponent<CharacterBody>();
                CharacterMaster master = cm.master;

                // Now searches all through keys
                if(masterPrefabNamesSummonReturn.Keys.Contains(self.masterPrefab.name))
                {
                    if (masterPrefabNamesSummonReturn[self.masterPrefab.name])
                    {
                        DII.checkConfig(characterMaster, master);
                        if(self.masterPrefab.name == "Drone2Master" || self.masterPrefab.name == "Emer")
                        {
                            characterMaster.inventory.ResetItem(ItemIndex.LunarPrimaryReplacement);
                        }
                    }
                }
                return characterMaster;
            };
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
                        self.Respawn(self.GetFieldValue<Vector3>("deathFootPosition"),
                            Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f), false);
                        self.gameObject.AddComponent<MasterSuicideOnTimer>().lifeTimer = DII.ConfigToFloat(DII.BackupDeathTimer.Value) + UnityEngine.Random.Range(0f, 3f);
                    }
                    else
                    {
                        self.Respawn(self.GetFieldValue<Vector3>("deathFootPosition"), Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f), false);
                    }
                };
            }
        }
        
    }
}
