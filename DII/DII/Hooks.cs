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
                            if(cm.bodyPrefab.name == "SquidTurretBody")
                            {
                                continue;
                            }
                            if (bodyprefabsDict[cm.bodyPrefab.name])
                            {
                                CharacterMaster owner = cm.gameObject.GetComponent<AIOwnership>().ownerMaster;
                                DII.updateInventory(cm, owner);
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
                    // I'm dumb and didn't realize this was an item for the ghost effect for over 5 months...
                    DII.customItem(cm, DII.CBItemGhosts.Value);
                    DII.customItemCap(cm, DII.CBItemCapGhosts.Value);
                    DII.customEquip(cm, DII.CBEquipGhosts.Value);
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
                                    
                                    DII.checkConfig(component, self.master);

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

                Inventory inventory = characterMaster.inventory;
                CharacterBody cm = activator.GetComponent<CharacterBody>();
                CharacterMaster master = cm.master;

                // Now searches all through keys
                if(masterPrefabNamesSummonReturn.Keys.Contains(self.masterPrefab.name))
                {
                    if (masterPrefabNamesSummonReturn[self.masterPrefab.name])
                    {
                        DII.checkConfig(characterMaster, master);
                        if(self.masterPrefab.name == "Drone2Master")
                        {
                            characterMaster.inventory.ResetItem(ItemIndex.LunarPrimaryReplacement);
                        }
                    }
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
                        self.gameObject.AddComponent<MasterSuicideOnTimer>().lifeTimer = DII.ConfigToFloat(DII.BackupDeathTimer.Value) + UnityEngine.Random.Range(0f, 3f);
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
                            EffectManager.SpawnEffect(gameObject, new EffectData
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
