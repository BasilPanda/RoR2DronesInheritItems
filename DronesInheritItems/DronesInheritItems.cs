using System;
using System.Linq;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using RoR2;
using UnityEngine;
using RoR2.CharacterAI;
using UnityEngine.Networking;

namespace Basil_ror2
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.Basil.DronesInheritItems", "DronesInheritItems", "2.2.0")]
    public class DroneWithItems : BaseUnityPlugin
    {
        public static ConfigWrapper<string> ItemMultiplier;
        public static ConfigWrapper<bool> ItemRandomizer;
        public static ConfigWrapper<bool> ItemGenerator;
        public static ConfigWrapper<bool> UpdateInventory;
        public static ConfigWrapper<string> Tier1GenCap;
        public static ConfigWrapper<string> Tier2GenCap;
        public static ConfigWrapper<string> Tier3GenCap;
        public static ConfigWrapper<string> LunarGenCap;
        public static ConfigWrapper<string> Tier1GenChance;
        public static ConfigWrapper<string> Tier2GenChance;
        public static ConfigWrapper<string> Tier3GenChance;
        public static ConfigWrapper<string> LunarGenChance;
        public static ConfigWrapper<string> EquipGenChance;
        public static ConfigWrapper<bool> LunarEquips;
        public static ConfigWrapper<bool> Tier1Items;
        public static ConfigWrapper<bool> Tier2Items;
        public static ConfigWrapper<bool> Tier3Items;
        public static ConfigWrapper<bool> LunarItems;
        public static ConfigWrapper<bool> EquipItems;
        public static ConfigWrapper<bool> InheritDio;
        public static ConfigWrapper<bool> InheritHappiestMask;
        public static ConfigWrapper<bool> FixBackupDio;
        public static ConfigWrapper<bool> DronesInherit;
        public static ConfigWrapper<bool> BackupDronesInherit;
        public static ConfigWrapper<bool> QueenGuardInherit;
        public static ConfigWrapper<bool> GhostInherit;

        public static EquipmentIndex[] LunarEquipmentList = new EquipmentIndex[]
        {
            EquipmentIndex.Meteor,
            EquipmentIndex.LunarPotion, // no idea what this is but it has lunar on it :D
            EquipmentIndex.BurnNearby,
            EquipmentIndex.CrippleWard
        };

        public static String[] bodyprefabNames = new String[]
        {
            "Drone1Body",
            "Drone2Body",
            "MegaDroneBody",
            "MissileDroneBody"
        };

        public void InitConfig()
        {
            DronesInherit = Config.Wrap(
                "Base Inherit Settings",
                "DronesInherit",
                "Toggles BOTH purchasable drones and turrets to inherit items.",
                true);

            BackupDronesInherit = Config.Wrap(
                "Base Inherit Settings",
                "BackupDronesInherit",
                "Toggles Backup Drones to inherit items.",
                true);

            ItemMultiplier = Config.Wrap(
                "Base Inherit Settings",
                "ItemMultiplier",
                "Sets the multiplier for items to be multiplied by when the drones inherit them." +
                "\nAble to handle decimal values now!",
                "1");

            ItemRandomizer = Config.Wrap(
                "Base Inherit Settings",
                "ItemRandomizer",
                "Toggles random amounts of items to be inherited from the player's inventory.",
                false
                );

            UpdateInventory = Config.Wrap(
                "Base Inherit Settings",
                "UpdateInventory",
                "Toggles updating drone inventory after every stage completion.",
                false
                );

            ItemGenerator = Config.Wrap(
                "Generator Settings",
                "ItemGenerator",
                "Toggles generation of items instead of inheriting items from player's inventory. WILL IGNORE BASE INHERIT SETTINGS IF TRUE.",
                false
                );

            Tier1GenCap = Config.Wrap(
                "Generator Settings",
                "Tier1GenCap",
                "The multiplicative max item cap for generating Tier 1 (white) items.",
                "4"
                );

            Tier2GenCap = Config.Wrap(
                "Generator Settings",
                "Tier2GenCap",
                "The multiplicative max item cap for generating Tier 2 (green) items.",
                "2"
                );

            Tier3GenCap = Config.Wrap(
                "Generator Settings",
                "Tier3GenCap",
                "The multiplicative max item cap for generating Tier 3 (red) items.",
                "1"
                );

            LunarGenCap = Config.Wrap(
                "Generator Settings",
                "LunarGenCap",
                "The multiplicative max item cap for generating Lunar (blue) items.",
                "1"
                );

            Tier1GenChance = Config.Wrap(
                "Generator Settings",
                "Tier1GenChance",
                "The percent chance for generating a Tier 1 (white) item.",
                "40"
                );

            Tier2GenChance = Config.Wrap(
                "Generator Settings",
                "Tier2GenChance",
                "The percent chance for generating a Tier 2 (green) item.",
                "20"
                );

            Tier3GenChance = Config.Wrap(
                "Generator Settings",
                "Tier3GenChance",
                "The percent chance for generating a Tier 3 (red) item.",
                "1"
                );

            LunarGenChance = Config.Wrap(
                "Generator Settings",
                "LunarGenChance",
                "The percent chance for generating a Lunar (blue) item.",
                "0.5"
                );

            EquipGenChance = Config.Wrap(
                "Generator Settings",
                "EquipGenChance",
                "The percent chance for generating a Use item.",
                "10"
                );

            Tier1Items = Config.Wrap(
                "General Settings",
                "Tier1Items",
                "Toggles Tier 1 (white) items to be inherited/generated.",
                true);

            Tier2Items = Config.Wrap(
                "General Settings",
                "Tier2Items",
                "Toggles Tier 2 (green) items to be inherited/generated.",
                true);

            Tier3Items = Config.Wrap(
                "General Settings",
                "Tier3Items",
                "Toggles Tier 3 (red) items to be inherited/generated.",
                true);

            LunarItems = Config.Wrap(
                "General Settings",
                "LunarItems",
                "Toggles Lunar (blue) items to be inherited/generated.",
                true);
            
            EquipItems = Config.Wrap(
                "General Settings",
                "EquipItems",
                "Toggles Use items to be inherited/generated. ONLY WORKS FOR QUEEN'S GUARD AND GHOSTS FROM HAPPIEST MASK.",
                false);

            LunarEquips = Config.Wrap(
                "General Settings",
                "LunarEquips",
                "Toggles Lunar Use items to be inherited/generated. ONLY WORKS FOR QUEEN'S GUARD AND GHOSTS FROM HAPPIEST MASK.",
                false);

            InheritDio = Config.Wrap(
                "General Settings",
                "InheritDio",
                "Toggles Dio's Best Friend to be inherited/generated by ALL types.",
                true);

            InheritHappiestMask = Config.Wrap(
                "General Settings",
                "InheritHappiestMask",
                "Toggles Happiest Mask to be inherited/generated by ALL types.",
                true);

            FixBackupDio = Config.Wrap(
                "General Settings",
                "FixBackupDio",
                "Makes it so that Backup drones will reinherit the 25 second death timer upon Dio revive.",
                true);

            QueenGuardInherit = Config.Wrap(
                "General Settings",
                "QueenGuardInherit",
                "Toggles Queen Guards to inherit/generate items.",
                false);

            GhostInherit = Config.Wrap(
                "General Settings",
                "GhostInherit",
                "Toggles ghosts spawned from Happiest Mask to inherit/generate items.\n" +
                "Dev notice: If ghosts create other ghosts, damage for the new ghost will\nbe multiplied " +
                "by 500% ON TOP of the original ghost 500% damage buff.\nThis cycle can continue non stop.",
                false);
           
        }

        public static float ConfigToFloat(string configline)
        {
            if(float.TryParse(configline,out float x))
            {
                return x;
            }
            return 1f;
        }

        public void Awake()
        {
            InitConfig();

            fixBackupDio();
            baseDrones();
            backupDrones();
            queensGuard();
            spookyGhosts();
            updateAfterStage();
        }     

        public static void checkConfig(Inventory inventory, CharacterMaster master)
        {
            if (ItemGenerator.Value) // Using generator instead
            {
                resetInventory(inventory);
                int scc = Run.instance.stageClearCount;
                if (Tier1Items.Value)
                {
                    foreach (ItemIndex index in ItemCatalog.tier1ItemList)
                    {
                        if (Util.CheckRoll(ConfigToFloat(Tier1GenCap.Value), master))
                        {
                            inventory.GiveItem(index, UnityEngine.Random.Range(0, (int)(scc * ConfigToFloat(Tier1GenCap.Value) + 1)));
                        }
                    }
                }
                if (Tier2Items.Value)
                {
                    foreach (ItemIndex index in ItemCatalog.tier2ItemList)
                    {
                        if (Util.CheckRoll(ConfigToFloat(Tier2GenCap.Value), master))
                        {
                            inventory.GiveItem(index, UnityEngine.Random.Range(0, (int)(scc * ConfigToFloat(Tier2GenCap.Value) + 1)));
                        }
                    }
                }
                if (Tier3Items.Value)
                {
                    foreach (ItemIndex index in ItemCatalog.tier3ItemList)
                    {
                        if (Util.CheckRoll(ConfigToFloat(Tier3GenCap.Value), master))
                        {
                            inventory.GiveItem(index, UnityEngine.Random.Range(0, (int)(scc * ConfigToFloat(Tier3GenCap.Value) + 1)));
                        }
                    }
                }
                if (LunarItems.Value)
                {
                    foreach (ItemIndex index in ItemCatalog.lunarItemList)
                    {
                        if (Util.CheckRoll(ConfigToFloat(LunarGenCap.Value), master))
                        {
                            inventory.GiveItem(index, UnityEngine.Random.Range(0, (int)(scc * ConfigToFloat(LunarGenCap.Value) + 1)));
                        }
                    }
                }
                if (!InheritDio.Value)
                {
                    inventory.ResetItem(ItemIndex.ExtraLife);
                }
                if (!InheritHappiestMask.Value)
                {
                    inventory.ResetItem(ItemIndex.GhostOnKill);
                }
                if (EquipItems.Value)
                {
                    if(Util.CheckRoll(ConfigToFloat(EquipGenChance.Value), master))
                    {
                        inventory.ResetItem(ItemIndex.AutoCastEquipment);
                        inventory.GiveItem(ItemIndex.AutoCastEquipment, 1);
                        EquipmentIndex equipmentIndex = Run.instance.availableEquipmentDropList[Run.instance.spawnRng.RangeInt(0, Run.instance.availableEquipmentDropList.Count)].equipmentIndex;
                        if (!LunarEquips.Value)
                        {
                            for (int i = 0; i < LunarEquipmentList.Length; i++)
                            {
                                if (equipmentIndex == LunarEquipmentList[i])
                                {
                                    equipmentIndex = EquipmentIndex.Fruit; // default to fruit.
                                    break;
                                }
                            }
                        }
                        inventory.SetEquipmentIndex(equipmentIndex);
                    }
                }
            }
            else // Default inheritance
            {
                updateInventory(inventory, master);
            }

            // Items that will never be used by the NPCs.
            inventory.ResetItem(ItemIndex.TreasureCache);
            inventory.ResetItem(ItemIndex.Feather);
            inventory.ResetItem(ItemIndex.Firework);
            inventory.ResetItem(ItemIndex.SprintArmor);
            inventory.ResetItem(ItemIndex.JumpBoost);
            inventory.ResetItem(ItemIndex.GoldOnHit);
            inventory.ResetItem(ItemIndex.WardOnLevel);
            inventory.ResetItem(ItemIndex.BeetleGland);
            inventory.ResetItem(ItemIndex.CrippleWardOnLevel);
        }

        public static void resetInventory(Inventory inventory)
        {
            foreach (ItemIndex index in ItemCatalog.tier1ItemList)
            {
                inventory.ResetItem(index);
            }
            foreach (ItemIndex index in ItemCatalog.tier2ItemList)
            {
                inventory.ResetItem(index);
            }
            foreach (ItemIndex index in ItemCatalog.tier3ItemList)
            {
                inventory.ResetItem(index);
            }
            foreach (ItemIndex index in ItemCatalog.lunarItemList)
            {
                inventory.ResetItem(index);
            }
        }

        public static void fixBackupDio()
        {
            if(FixBackupDio.Value)
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

        public static void baseDrones()
        {
           if (DronesInherit.Value)
            {
                On.RoR2.SummonMasterBehavior.OpenSummon += (orig, self, activator) =>
                {
                    if (!NetworkServer.active)
                    {
                        Debug.LogWarning("[Server] function 'System.Void RoR2.SummonMasterBehavior::OpenSummon(RoR2.Interactor)' called on client");
                        return;
                    }
                    GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(self.masterPrefab, self.transform.position, self.transform.rotation);
                    CharacterBody component = activator.GetComponent<CharacterBody>();
                    CharacterMaster master = component.master;
                    CharacterMaster component2 = gameObject.GetComponent<CharacterMaster>();
                    component2.teamIndex = TeamComponent.GetObjectTeam(component.gameObject);
                    Inventory component3 = gameObject.GetComponent<Inventory>();
             
                    component3.CopyItemsFrom(master.inventory);
                    checkConfig(component3, master);
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
                };
            }
        }

        public static void backupDrones()
        {
            if(BackupDronesInherit.Value)
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

                    checkConfig(component3, master);

                    if (EquipItems.Value)
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

        public static void queensGuard()
        {
            if (QueenGuardInherit.Value)
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
                                    checkConfig(inventory, self.master);

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

        public static void spookyGhosts()
        {
            if (GhostInherit.Value)
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
                    checkConfig(component.inventory, ownerBody.master);
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

        public static void updateInventory(Inventory inventory, CharacterMaster master)
        {
            if (!Tier1Items.Value)
            {
                foreach (ItemIndex index in ItemCatalog.tier1ItemList)
                {
                    inventory.ResetItem(index);
                }
            }
            if (!Tier2Items.Value)
            {
                foreach (ItemIndex index in ItemCatalog.tier2ItemList)
                {
                    inventory.ResetItem(index);
                }
            }
            if (!Tier3Items.Value)
            {
                foreach (ItemIndex index in ItemCatalog.tier3ItemList)
                {
                    inventory.ResetItem(index);
                }
            }
            if (!LunarItems.Value)
            {
                foreach (ItemIndex index in ItemCatalog.lunarItemList)
                {
                    inventory.ResetItem(index);
                }
            }
            if (!InheritDio.Value)
            {
                inventory.ResetItem(ItemIndex.ExtraLife);
            }
            if (!InheritHappiestMask.Value)
            {
                inventory.ResetItem(ItemIndex.GhostOnKill);
            }
            float itemMultiplier = ConfigToFloat(ItemMultiplier.Value);
            if (itemMultiplier != 1f)
            {
                int count = 0;
                if (Tier1Items.Value)
                {
                    foreach (ItemIndex index in ItemCatalog.tier1ItemList)
                    {
                        count = inventory.GetItemCount(index);
                        inventory.ResetItem(index);
                        inventory.GiveItem(index, (int)Math.Ceiling(count * itemMultiplier));
                    }
                }
                if (Tier2Items.Value)
                {
                    foreach (ItemIndex index in ItemCatalog.tier2ItemList)
                    {
                        count = inventory.GetItemCount(index);
                        inventory.ResetItem(index);
                        inventory.GiveItem(index, (int)Math.Ceiling(count * itemMultiplier));
                    }
                }
                if (Tier3Items.Value)
                {
                    foreach (ItemIndex index in ItemCatalog.tier3ItemList)
                    {
                        count = inventory.GetItemCount(index);
                        inventory.ResetItem(index);
                        inventory.GiveItem(index, (int)Math.Ceiling(count * itemMultiplier));
                    }
                }
                if (LunarItems.Value)
                {
                    foreach (ItemIndex index in ItemCatalog.lunarItemList)
                    {
                        count = inventory.GetItemCount(index);
                        inventory.ResetItem(index);
                        inventory.GiveItem(index, (int)Math.Ceiling(count * itemMultiplier));
                    }
                }
            }

            if (ItemRandomizer.Value)
            {
                int count = 0;
                if (Tier1Items.Value)
                {
                    foreach (ItemIndex index in ItemCatalog.tier1ItemList)
                    {
                        count = inventory.GetItemCount(index);
                        inventory.ResetItem(index);
                        inventory.GiveItem(index, UnityEngine.Random.Range(0, count + 1));
                    }
                }
                if (Tier2Items.Value)
                {
                    foreach (ItemIndex index in ItemCatalog.tier2ItemList)
                    {
                        count = inventory.GetItemCount(index);
                        inventory.ResetItem(index);
                        inventory.GiveItem(index, UnityEngine.Random.Range(0, count + 1));
                    }
                }
                if (Tier3Items.Value)
                {
                    foreach (ItemIndex index in ItemCatalog.tier3ItemList)
                    {
                        count = inventory.GetItemCount(index);
                        inventory.ResetItem(index);
                        inventory.GiveItem(index, UnityEngine.Random.Range(0, count + 1));
                    }
                }
                if (LunarItems.Value)
                {
                    foreach (ItemIndex index in ItemCatalog.lunarItemList)
                    {
                        count = inventory.GetItemCount(index);
                        inventory.ResetItem(index);
                        inventory.GiveItem(index, UnityEngine.Random.Range(0, count + 1));
                    }
                }
            }

            if (EquipItems.Value)
            {
                inventory.ResetItem(ItemIndex.AutoCastEquipment);
                inventory.GiveItem(ItemIndex.AutoCastEquipment, 1);
                inventory.CopyEquipmentFrom(master.inventory);

                if (!LunarEquips.Value)
                {
                    for (int i = 0; i < LunarEquipmentList.Length; i++)
                    {
                        if (inventory.GetEquipmentIndex() == LunarEquipmentList[i])
                        {
                            inventory.SetEquipmentIndex(EquipmentIndex.Fruit); // default to fruit
                            break;
                        }
                    }
                }
            }
        }

        public static void updateAfterStage()
        {
            if (UpdateInventory.Value)
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
                            updateInventory(inventory, host);
                          
                        }
                    }
                    orig(self, nextSceneName);
                };
            }
        }
    }
}
