using System;
using System.Linq;
using System.Collections.Generic;
using RoR2;
using UnityEngine;
using RoR2.CharacterAI;
using UnityEngine.Networking;

namespace Basil_ror2
{
    public static class Hooks
    {
        // Bodyprefabs to check
        public static String[] bodyprefabNames = new String[]
        {
            "Drone1Body",
            "Drone2Body",
            "MegaDroneBody",
            "MissileDroneBody"
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
            "TitanGoldMaster"
        };

        // Update inventories after every stage -- NO SOURCE CHANGE
        public static void updateAfterStage()
        {
            if (DII.UpdateInventory.Value)
            {
                On.RoR2.Run.AdvanceStage += (orig, self, nextSceneName) =>
                {
                    var masters = CharacterMaster.readOnlyInstancesList;
                    List<CharacterMaster> masterList = masters.Cast<CharacterMaster>().ToList();
                    foreach (CharacterMaster cm in masterList)
                    {
                        if (bodyprefabNames.Contains(cm.bodyPrefab.name))
                        {
                            CharacterMaster host = PlayerCharacterMasterController.instances[0].master;
                            Inventory inventory = cm.inventory;
                            inventory.CopyItemsFrom(host.inventory);
                            DII.updateInventory(inventory, host);

                        }
                    }
                    orig(self, nextSceneName);
                };
            }
        }

        // Ghost inheritance -- NO LONGER NEEDED, CODE REFACTORED TO MASTER SUMMON
        public static void spookyGhosts()
        {
            if (DII.GhostInherit.Value)
            {
                On.RoR2.Util.TryToCreateGhost += (orig, targetBody, ownerBody, duration) =>
                {
                    if (!targetBody || !NetworkServer.active)
                    {
                        return null;
                    }
                    if (TeamComponent.GetTeamMembers(ownerBody.teamComponent.teamIndex).Count >= 40)
                    {
                        return null;
                    }
                    int num = BodyCatalog.FindBodyIndex(targetBody.gameObject);
                    if (num < 0)
                    {
                        return null;
                    }
                    GameObject bodyPrefab = BodyCatalog.GetBodyPrefab(num);
                    if (!bodyPrefab)
                    {
                        return null;
                    }
                    CharacterMaster characterMaster = MasterCatalog.allAiMasters.FirstOrDefault((CharacterMaster master) => master.bodyPrefab == bodyPrefab);
                    if (!characterMaster)
                    {
                        return null;
                    }
                    GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(characterMaster.gameObject);
                    CharacterMaster component = gameObject.GetComponent<CharacterMaster>();
                    component.teamIndex = ownerBody.teamComponent.teamIndex;
                    component.GetComponent<BaseAI>().leader.gameObject = ownerBody.gameObject;
                    Inventory inventory = targetBody.inventory;
                    if (inventory)
                    {
                        component.inventory.CopyItemsFrom(ownerBody.inventory);
                    }
                    DII.checkConfig(component.inventory, ownerBody.master);
                    component.inventory.GiveItem(ItemIndex.Ghost, 1);
                    component.inventory.GiveItem(ItemIndex.HealthDecay, duration);
                    component.inventory.GiveItem(ItemIndex.BoostDamage, 30);
                    NetworkServer.Spawn(gameObject);
                    CharacterBody characterBody = component.Respawn(targetBody.footPosition, targetBody.transform.rotation, false);
                    if (characterBody)
                    {
                        foreach (EntityStateMachine entityStateMachine in characterBody.GetComponents<EntityStateMachine>())
                        {
                            entityStateMachine.initialStateType = entityStateMachine.mainStateType;
                        }
                    }
                    return characterBody;
                };
            }
        }

        // Queen Guard inheritance -- SOURCE CODE CHANGED -- UPDATED MODDED CODE
        public static void queensGuard()
        {
            if (DII.QueenGuardInherit.Value)
            {
                On.RoR2.CharacterBody.UpdateBeetleGuardAllies += (orig, self) =>
                {
                    if (NetworkServer.active)
                    {
                        int num = self.inventory ? self.inventory.GetItemCount(ItemIndex.BeetleGland) : 0;
                        if (num > 0 && self.master.GetDeployableCount(DeployableSlot.BeetleGuardAlly) < num)
                        {
                            self.SetFieldValue("guardResummonCooldown", self.GetFieldValue<float>("guardResummonCooldown") - Time.fixedDeltaTime);

                            if (self.GetFieldValue<float>("guardResummonCooldown") <= 0f)
                            {
                                self.SetFieldValue("guardResummonCooldown", 30f);
                                DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest((SpawnCard)Resources.Load("SpawnCards/CharacterSpawnCards/cscBeetleGuardAlly"), new DirectorPlacementRule
                                {
                                    placementMode = DirectorPlacementRule.PlacementMode.Approximate,
                                    minDistance = 3f,
                                    maxDistance = 40f,
                                    spawnOnTarget = self.transform
                                }, RoR2Application.rng);
                                directorSpawnRequest.summonerBodyObject = self.gameObject;
                                GameObject gameObject = DirectorCore.instance.TrySpawnObject(directorSpawnRequest);
                                if (gameObject)
                                {
                                    CharacterMaster component = gameObject.GetComponent<CharacterMaster>();
                                    Inventory inventory = gameObject.GetComponent<Inventory>();

                                    inventory.CopyItemsFrom(self.inventory);
                                    DII.checkConfig(inventory, self.master);

                                    AIOwnership component2 = gameObject.GetComponent<AIOwnership>();
                                    BaseAI component3 = gameObject.GetComponent<BaseAI>();
                                    if (component)
                                    {
                                        component.teamIndex = TeamComponent.GetObjectTeam(self.gameObject);
                                        component.inventory.GiveItem(ItemIndex.BoostDamage, 30);
                                        component.inventory.GiveItem(ItemIndex.BoostHp, 10);
                                        GameObject bodyObject = component.GetBodyObject();
                                        if (bodyObject)
                                        {
                                            Deployable component4 = bodyObject.GetComponent<Deployable>();
                                            self.master.AddDeployable(component4, DeployableSlot.BeetleGuardAlly);
                                        }
                                    }
                                    if (component2)
                                    {
                                        component2.ownerMaster = self.master;
                                    }
                                    if (component3)
                                    {
                                        component3.leader.gameObject = self.gameObject;
                                    }
                                }
                            }
                        }
                    }
                };
            }
        }

        // Backup Drone inheritance --  NO LONGER NEEDED, CODE REFACTORED TO MASTER SUMMON
        public static void backupDrones()
        {
            if (DII.BackupDronesInherit.Value)
            {
                On.RoR2.EquipmentSlot.SummonMaster += (orig, self, masterObjectPrefab, position, rotation) =>
                {
                    if (!NetworkServer.active)
                    {
                        Debug.LogWarning("[Server] function 'RoR2.CharacterMaster RoR2.EquipmentSlot::SummonMaster(UnityEngine.GameObject,UnityEngine.Vector3,UnityEngine.Quaternion)' called on client");
                        return null;
                    }
                    CharacterMaster characterMaster = new MasterSummon
                    {
                        masterPrefab = masterObjectPrefab,
                        position = position,
                        rotation = rotation,
                        summonerBodyObject = self.gameObject,
                        ignoreTeamMemberLimit = false
                    }.Perform();
                    Inventory inventory = characterMaster.inventory;
                    Chat.AddMessage(masterObjectPrefab.name);
                    if (DII.BackupDronesInherit.Value && masterObjectPrefab.name == "DroneBackupMaster")
                    {
                        inventory.CopyItemsFrom(characterMaster.gameObject.GetComponent<AIOwnership>().ownerMaster.inventory);
                        DII.checkConfig(inventory, characterMaster.gameObject.GetComponent<AIOwnership>().ownerMaster);
                    }
                    return characterMaster;
                };
            }
            
        }

        // All Drones inheritance
        public static void baseDrones()
        {
            On.RoR2.MasterSummon.Perform += (orig, self) =>
            {
                TeamIndex teamIndex;
                if (self.teamIndexOverride != null)
                {
                    teamIndex = self.teamIndexOverride.Value;
                }
                else
                {
                    if (!self.summonerBodyObject)
                    {
                        Debug.LogErrorFormat("Cannot spawn master {0}: No team specified.", new object[]
                        {
                        self.masterPrefab
                        });
                        return null;
                    }
                    teamIndex = TeamComponent.GetObjectTeam(self.summonerBodyObject);
                }
                if (!self.ignoreTeamMemberLimit)
                {
                    TeamDef teamDef = TeamCatalog.GetTeamDef(teamIndex);
                    if (teamDef == null)
                    {
                        Debug.LogErrorFormat("Attempting to spawn master {0} on TeamIndex.None. Is this intentional?", new object[]
                        {
                        self.masterPrefab
                        });
                        return null;
                    }
                    if (teamDef != null && teamDef.softCharacterLimit <= TeamComponent.GetTeamMembers(teamIndex).Count)
                    {
                        return null;
                    }
                }
                GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(self.masterPrefab, self.position, self.rotation);
                CharacterMaster component = gameObject.GetComponent<CharacterMaster>();
                component.teamIndex = teamIndex;
               
                if (self.summonerBodyObject)
                {
                    AIOwnership component2 = gameObject.GetComponent<AIOwnership>();
                    if (component2)
                    {
                        CharacterBody component3 = self.summonerBodyObject.GetComponent<CharacterBody>();
                        if (component3)
                        {
                            CharacterMaster master = component3.master;
                            if (master)
                            {
                                component2.ownerMaster = master;

                                /////////////////
                                /////////////////
                                /////////////////
                                /////////////////
                                // MODDED PART //
                                /////////////////
                                /////////////////
                                /////////////////
                                /////////////////
                                
                                Inventory inventory = gameObject.GetComponent<Inventory>();

                                // Gunner drones
                                if (DII.GunnerDronesInherit.Value && self.masterPrefab.name.ToString() == "Drone1Master")
                                {
                                    inventory.CopyItemsFrom(master.inventory);
                                    DII.checkConfig(inventory, master);
                                }
                                // Healer drones
                                else if (DII.HealDronesInherit.Value && self.masterPrefab.name.ToString() == "Drone2Master")
                                {
                                    inventory.CopyItemsFrom(master.inventory);
                                    DII.checkConfig(inventory, master);
                                }
                                // Missile drones
                                else if (DII.MissileDronesInherit.Value && self.masterPrefab.name.ToString() == "DroneMissileMaster")
                                {
                                    inventory.CopyItemsFrom(master.inventory);
                                    DII.checkConfig(inventory, master);
                                }
                                // TC-280 Prototype drones
                                else if (DII.ProtoDronesInherit.Value && self.masterPrefab.name.ToString() == "MegaDroneMaster")
                                {
                                    inventory.CopyItemsFrom(master.inventory);
                                    DII.checkConfig(inventory, master);
                                }
                                // Incinerator drones
                                else if (DII.FlameDronesInherit.Value && self.masterPrefab.name.ToString() == "FlameDroneMaster")
                                {
                                    inventory.CopyItemsFrom(master.inventory);
                                    DII.checkConfig(inventory, master);
                                }
                                // Equipment drones
                                else if (DII.EquipDronesInherit.Value && self.masterPrefab.name.ToString() == "EquipmentDroneMaster")
                                {
                                    inventory.CopyItemsFrom(master.inventory);
                                    DII.checkConfig(inventory, master);
                                }
                                /*
                                // All turrets
                                else if (DII.TurretsInherit.Value && self.masterPrefab.name.ToString() == "Turret1Master")
                                {
                                    Chat.AddMessage("Turret start");
                                    inventory.CopyItemsFrom(master.inventory);
                                    Chat.AddMessage("Turret middle");
                                    DII.checkConfig(inventory, master);
                                    Chat.AddMessage("Turret end");
                                }
                                */
                                /////////////////
                                /////////////////
                                /////////////////
                                /////////////////
                                // MODDED PART //
                                /////////////////
                                /////////////////
                                /////////////////
                                /////////////////

                            }
                        }
                    }
                    BaseAI component4 = gameObject.GetComponent<BaseAI>();
                    if (component4)
                    {
                        component4.leader.gameObject = self.summonerBodyObject;
                    }
                }
                Action<CharacterMaster> action = self.preSpawnSetupCallback;
                if (action != null)
                {
                    action(component);

                    /////////////////
                    /////////////////
                    /////////////////
                    /////////////////
                    // MODDED PART //
                    /////////////////
                    /////////////////
                    /////////////////
                    /////////////////

                    // Spooky Ghosts
                    Inventory inventory = gameObject.GetComponent<Inventory>();
                    if (DII.GhostInherit.Value)
                    {
                        AIOwnership component2 = gameObject.GetComponent<AIOwnership>();
                        inventory.CopyItemsFrom(component2.ownerMaster.inventory);
                        DII.checkConfig(inventory, component2.ownerMaster);
                    }
                }
                NetworkServer.Spawn(gameObject);
                component.Respawn(self.position, self.rotation, false);
                return component;
            };
        }
        
        // Turrets
        public static void turrets()
        {
            On.RoR2.SummonMasterBehavior.OpenSummonReturnMaster += (orig, self, activator) =>
            {

                if (!NetworkServer.active)
                {
                    Debug.LogWarning("[Server] function 'RoR2.CharacterMaster RoR2.SummonMasterBehavior::OpenSummonReturnMaster(RoR2.Interactor)' called on client");
                    return null;
                }
                float d = 0f;
                CharacterMaster characterMaster = new MasterSummon
                {
                    masterPrefab = self.masterPrefab,
                    position = self.transform.position + Vector3.up * d,
                    rotation = self.transform.rotation,
                    summonerBodyObject = ((activator != null) ? activator.gameObject : null),
                    ignoreTeamMemberLimit = true
                }.Perform();
                Chat.AddMessage(self.masterPrefab.name);
                if (DII.TurretsInherit.Value && self.masterPrefab.name == "Turret1Master")
                {
                    Inventory inventory = characterMaster.inventory;
                    CharacterBody cm = activator.GetComponent<CharacterBody>();
                    CharacterMaster master = cm.master;
                    inventory.CopyItemsFrom(master.inventory);
                }
                if (characterMaster)
                {
                    GameObject bodyObject = characterMaster.GetBodyObject();
                    if (bodyObject)
                    {
                        ModelLocator component = bodyObject.GetComponent<ModelLocator>();
                        if (component && component.modelTransform)
                        {
                            TemporaryOverlay temporaryOverlay = component.modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                            temporaryOverlay.duration = 0.5f;
                            temporaryOverlay.animateShaderAlpha = true;
                            temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                            temporaryOverlay.destroyComponentOnEnd = true;
                            temporaryOverlay.originalMaterial = Resources.Load<Material>("Materials/matSummonDrone");
                            temporaryOverlay.AddToCharacerModel(component.modelTransform.GetComponent<CharacterModel>());
                        }
                    }
                }
                if (self.destroyAfterSummoning)
                {
                    UnityEngine.Object.Destroy(self.gameObject);
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
                    self.inventory.GiveItem(ItemIndex.ExtraLifeConsumed, 1);

                    if (self.bodyPrefab.name.ToString() == "BackupDroneBody")
                    {
                        self.Respawn(self.GetFieldValue<Vector3>("deathFootPosition"),
                            Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f), false);
                        self.gameObject.AddComponent<MasterSuicideOnTimer>().lifeTimer = 25f + UnityEngine.Random.Range(0f, 3f);
                    }
                    else
                    {
                        self.Respawn(self.GetFieldValue<Vector3>("deathFootPosition"), Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f), false);
                    }
                    self.GetBody().AddTimedBuff(BuffIndex.Immune, 3f);
                    GameObject gameObject = Resources.Load<GameObject>("Prefabs/Effects/HippoRezEffect");
                    if (self.GetPropertyValue<GameObject>("bodyInstanceObject"))
                    {
                        foreach (EntityStateMachine entityStateMachine in self.GetPropertyValue<GameObject>("bodyInstanceObject").GetComponents<EntityStateMachine>())
                        {
                            entityStateMachine.initialStateType = entityStateMachine.mainStateType;
                        }
                        if (gameObject)
                        {
                            EffectManager.instance.SpawnEffect(gameObject, new EffectData
                            {
                                origin = self.GetFieldValue<Vector3>("deathFootPosition"),
                                rotation = self.GetPropertyValue<GameObject>("bodyInstanceObject").transform.rotation
                            }, true);
                        }
                    }
                };
            }
        }
    }
}
