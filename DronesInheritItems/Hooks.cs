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

        // Update inventories after every stage
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

        // Ghost inheritance
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

        // Queen Guard inheritance
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
                                GameObject gameObject = DirectorCore.instance.TrySpawnObject((SpawnCard)Resources.Load("SpawnCards/CharacterSpawnCards/cscBeetleGuardAlly"), new DirectorPlacementRule
                                {
                                    placementMode = DirectorPlacementRule.PlacementMode.Approximate,
                                    minDistance = 3f,
                                    maxDistance = 40f,
                                    spawnOnTarget = self.transform
                                }, RoR2Application.rng);
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

        // Backup Drone inheritance
        public static void backupDrones()
        {
            if (DII.BackupDronesInherit.Value)
            {
                On.RoR2.EquipmentSlot.SummonMaster += (orig, self, masterObjectPrefab, position) =>
                {
                    if (!NetworkServer.active)
                    {
                        Debug.LogWarning("[Server] function 'RoR2.CharacterMaster RoR2.EquipmentSlot::SummonMaster(UnityEngine.GameObject,UnityEngine.Vector3)' called on client");
                        return null;
                    }
                    GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(masterObjectPrefab, position, self.transform.rotation);
                    CharacterBody component = self.GetComponent<CharacterBody>();
                    CharacterMaster master = component.master;
                    CharacterMaster component2 = gameObject.GetComponent<CharacterMaster>();
                    component2.teamIndex = TeamComponent.GetObjectTeam(component.gameObject);
                    Inventory component3 = gameObject.GetComponent<Inventory>();
                    component3.CopyItemsFrom(master.inventory);

                    DII.checkConfig(component3, master);

                    if (DII.EquipItems.Value)
                    {
                        if (component3.GetEquipmentIndex() == EquipmentIndex.DroneBackup)
                        {
                            component3.SetEquipmentIndex(EquipmentIndex.Fruit); // default to fruit. make this a config option next patch or something
                        }
                    }

                    NetworkServer.Spawn(gameObject);
                    component2.SpawnBody(component2.bodyPrefab, position, self.transform.rotation);
                    AIOwnership component4 = gameObject.GetComponent<AIOwnership>();
                    if (component4 && component && master)
                    {
                        component4.ownerMaster = master;
                    }
                    BaseAI component5 = gameObject.GetComponent<BaseAI>();
                    if (component5)
                    {
                        component5.leader.gameObject = self.gameObject;
                    }
                    return component2;
                };
            }
        }

        // Drones & Turrets inheritance
        public static void baseDrones()
        {
            On.RoR2.SummonMasterBehavior.OpenSummon += (orig, self, activator) =>
            {
                if (!NetworkServer.active)
                {
                    Debug.LogWarning("[Server] function 'System.Void RoR2.SummonMasterBehavior::OpenSummon(RoR2.Interactor)' called on client");
                    return;
                }
                if (DII.DronesInherit.Value || DII.TurretsInherit.Value)
                { 
                    GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(self.masterPrefab, self.transform.position, self.transform.rotation);
                    CharacterBody component = activator.GetComponent<CharacterBody>();
                    CharacterMaster master = component.master;
                    CharacterMaster component2 = gameObject.GetComponent<CharacterMaster>();
                    component2.teamIndex = TeamComponent.GetObjectTeam(component.gameObject);
                    Inventory component3 = gameObject.GetComponent<Inventory>();
                    if (DII.DronesInherit.Value && self.masterPrefab.name.ToString() != "Turret1Master")
                    {
                        component3.CopyItemsFrom(master.inventory);
                        DII.checkConfig(component3, master);
                    }
                    else if (DII.TurretsInherit.Value && self.masterPrefab.name.ToString() == "Turret1Master")
                    {
                        component3.CopyItemsFrom(master.inventory);
                        DII.checkConfig(component3, master);
                    }
                    NetworkServer.Spawn(gameObject);
                    component2.SpawnBody(component2.bodyPrefab, self.transform.position + Vector3.up * 0.8f, self.transform.rotation);
                    AIOwnership component4 = gameObject.GetComponent<AIOwnership>();
                    if (component4 && component && master)
                    {
                        component4.ownerMaster = master;
                    }
                    BaseAI component5 = gameObject.GetComponent<BaseAI>();
                    if (component5)
                    {
                        component5.leader.gameObject = activator.gameObject;
                    }
                    UnityEngine.Object.Destroy(self.gameObject);
                }
                else
                {
                    orig(self, activator);
                }

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
