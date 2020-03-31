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
        // Bodyprefabs to check
        public static String[] bodyprefabNames = new String[]
        {
            "Drone1Body",
            "Drone2Body",
            "MegaDroneBody",
            "MissileDroneBody",
            "FlameDroneBody",
            "EquipmentDroneBody"
        };

        // Config Drone Values
        public static Dictionary<string, bool> dict = new Dictionary<string, bool>()
        {
            {"Drone1Body", DII.GunnerDronesInherit.Value},
            {"Drone2Body", DII.HealDronesInherit.Value},
            {"MegaDroneBody", DII.ProtoDronesInherit.Value},
            {"MissileDroneBody", DII.MissileDronesInherit.Value},
            {"FlameDroneBody", DII.FlameDronesInherit.Value},
            {"EquipmentDroneBody", DII.EquipDronesInherit.Value},
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
                            if (dict[cm.bodyPrefab.name])
                            {
                                CharacterMaster owner = cm.gameObject.GetComponent<AIOwnership>().ownerMaster;
                                Inventory inventory = cm.inventory;
                                inventory.CopyItemsFrom(owner.inventory);
                                DII.updateInventory(inventory, owner);
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
                    CharacterMaster cm = characterBody.master;
                    Inventory inventory = cm.inventory;
                    AIOwnership component2 = cm.gameObject.GetComponent<AIOwnership>();
                    CharacterMaster master = ownerBody.master;
                    inventory.CopyItemsFrom(master.inventory);
                    DII.checkConfig(inventory, master);
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
                    if (DII.BackupDronesInherit.Value && masterObjectPrefab.name == "DroneBackupMaster")
                    {
                        inventory.CopyItemsFrom(characterMaster.gameObject.GetComponent<AIOwnership>().ownerMaster.inventory);
                        DII.checkConfig(inventory, characterMaster.gameObject.GetComponent<AIOwnership>().ownerMaster);
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
                Debug.Log("CM Name: " + titan.ToString() + "\nBody Name:" + titan.GetBody().name);
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
                if (DII.GoldTitanInherit.Value && cm != null)
                {
                    titan.inventory.CopyItemsFrom(cm.inventory);
                    DII.checkConfig(titan.inventory, cm);
                }
                /*
                if (!NetworkServer.active)
                {
                    return;
                }
                int num = 0;
                CharacterMaster cm = null;
                ReadOnlyCollection<TeamComponent> teamMembers = TeamComponent.GetTeamMembers(TeamIndex.Player);
                for (int i = 0; i < teamMembers.Count; i++)
                {
                    if (Util.LookUpBodyNetworkUser(teamMembers[i].gameObject))
                    {
                        CharacterBody component = teamMembers[i].GetComponent<CharacterBody>();
                        if (component && component.inventory)
                        {
                            num += component.inventory.GetItemCount(ItemIndex.TitanGoldDuringTP);
                            cm = component.master;
                        }
                    }
                }
                if (num > 0)
                {
                    DirectorPlacementRule placementRule = new DirectorPlacementRule
                    {
                        placementMode = DirectorPlacementRule.PlacementMode.NearestNode,
                        minDistance = 20f,
                        maxDistance = 130f,
                        position = self.transform.position
                    };
                    DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest(Resources.Load<SpawnCard>("SpawnCards/CharacterSpawnCards/cscTitanGoldAlly"), placementRule, self.GetFieldValue<Xoroshiro128Plus>("rng"));
                    directorSpawnRequest.ignoreTeamMemberLimit = true;
                    directorSpawnRequest.teamIndexOverride = new TeamIndex?(TeamIndex.Player);
                    GameObject gameObject = DirectorCore.instance.TrySpawnObject(directorSpawnRequest);
                    if (gameObject)
                    {
                        float num2 = 1f;
                        float num3 = 1f;
                        num3 += Run.instance.difficultyCoefficient / 8f;
                        num2 += Run.instance.difficultyCoefficient / 2f;
                        CharacterMaster component2 = gameObject.GetComponent<CharacterMaster>();
                        self.SetFieldValue("titanGoldBossBody", component2.GetBody());
                        int livingPlayerCount = Run.instance.livingPlayerCount;
                        num2 *= Mathf.Pow((float)num, 1f);
                        num3 *= Mathf.Pow((float)num, 0.5f);
                        component2.inventory.GiveItem(ItemIndex.BoostHp, Mathf.RoundToInt((num2 - 1f) * 10f));
                        component2.inventory.GiveItem(ItemIndex.BoostDamage, Mathf.RoundToInt((num3 - 1f) * 10f));
                        
                        if (DII.GoldTitanInherit.Value && component2.GetBody().name == "TitanGoldBody(Clone)" && cm != null)
                        {
                            component2.inventory.CopyItemsFrom(cm.inventory);
                            DII.checkConfig(component2.inventory, cm);
                        }

                    }
                }*/
            };

        }
        

        // Squid Turrets
        public static void squidInherit()
        {
            IL.RoR2.GlobalEventManager.OnInteractionBegin += il =>
            {
                var c = new ILCursor(il);
                c.GotoNext(
                    MoveType.After,
                    i => i.MatchCallvirt("UnityEngine.GameObject", "GetComponent"),
                    i => i.MatchDup()
                    );
                //c.Emit(OpCodes.Ldarg_1);
                c.EmitDelegate<Action<Interactor, CharacterMaster>>((interactor, component) =>
                {
                    if (DII.SquidTurretsInherit.Value)
                    {
                        CharacterMaster playerMaster = interactor.GetComponent<CharacterMaster>();
                        Inventory playerInventory = playerMaster.inventory;
                        component.inventory.CopyItemsFrom(playerInventory);
                        DII.checkConfig(component.inventory, playerMaster);
                    }
                });
            };
            /*
            On.RoR2.GlobalEventManager.OnInteractionBegin += (orig, self, interactor, interactable, interactableObject) =>
            {

            };
            */
        }

        // Drones and Turrets
        public static void baseMod()
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

                Inventory inventory = characterMaster.inventory;
                CharacterBody cm = activator.GetComponent<CharacterBody>();
                CharacterMaster master = cm.master;
                // Minigun turrets
                if (DII.MinigunTurretsInherit.Value && self.masterPrefab.name == "Turret1Master")
                {
                    inventory.CopyItemsFrom(master.inventory);
                    DII.checkConfig(inventory, master);
                }
                // Gunner drones
                else if (DII.GunnerDronesInherit.Value && self.masterPrefab.name.ToString() == "Drone1Master")
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
                // Emergency drones
                else if (DII.EquipDronesInherit.Value && self.masterPrefab.name.ToString() == "EmergencyDroneMaster")
                {
                    inventory.CopyItemsFrom(master.inventory);
                    DII.checkConfig(inventory, master);
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
