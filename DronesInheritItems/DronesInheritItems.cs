using System;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using RoR2;
using UnityEngine;
using RoR2.CharacterAI;
using UnityEngine.Networking;

namespace Basil_ror2
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.Basil.DronesInheritItems", "DronesInheritItems", "2.0.0")]
    public class DroneWithItems : BaseUnityPlugin
    {
        public static ConfigWrapper<int> ItemMultiplier;
        public static ConfigWrapper<bool> Tier1Items;
        public static ConfigWrapper<bool> Tier2Items;
        public static ConfigWrapper<bool> Tier3Items;
        public static ConfigWrapper<bool> LunarItems;
        // public static ConfigWrapper<bool> EquipItems;
        public static ConfigWrapper<bool> InheritDio;
        public static ConfigWrapper<bool> QueenGuardInherit;
        public static ConfigWrapper<bool> GhostInherit;
        public static ConfigWrapper<bool> InheritHappiestMask;

        public void InitConfig()
        {
            ItemMultiplier = Config.Wrap(
                "Settings",
                "ItemMultiplier",
                "Sets the multiplier for items to be multiplied by when the drones inherit them." +
                "\nCurrently only able to multiple by integer values greater than or equal to 1.",
                1);

            Tier1Items = Config.Wrap(
                "Settings",
                "Tier1Items",
                "Toggles Tier 1 (white) items to be inherited.",
                true);

            Tier2Items = Config.Wrap(
                "Settings",
                "Tier2Items",
                "Toggles Tier 2 (green) items to be inherited.",
                true);

            Tier3Items = Config.Wrap(
                "Settings",
                "Tier3Items",
                "Toggles Tier 3 (red) items to be inherited.",
                true);

            LunarItems = Config.Wrap(
                "Settings",
                "LunarItems",
                "Toggles Lunar (blue) items to be inherited.",
                true);
            
            /* 
             * Mobs don't seem to use the equipments?
            EquipItems = Config.Wrap(
                "Settings",
                "EquipItems",
                "Toggles Use items to be inherited.",
                false);
            */

            InheritDio = Config.Wrap(
                "Settings",
                "InheritDio",
                "Toggles Dio's Best Friend to be inherited.",
                true);

            InheritHappiestMask = Config.Wrap(
                "Settings",
                "InheritHappiestMask",
                "Toggles Happiest Mask to be inherited.",
                true);

            QueenGuardInherit = Config.Wrap(
                "Settings",
                "QueenGuardInherit",
                "Toggles Queen Guards to inherit items.",
                false);

            GhostInherit = Config.Wrap(
                "Settings",
                "GhostInherit",
                "Toggles ghosts spawned from Happiest Mask to inherit items.\n" +
                "Dev notice: If ghosts create other ghosts, damage for the new ghost will\nbe multiplied " +
                "by 500% ON TOP of the original ghost 500% damage buff.\nThis cycle can continue non stop.",
                false);
           

        }

        public void Awake()
        {
            InitConfig();
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
                /*
                if (EquipItems.Value)
                {
                    if (master.inventory.GetEquipmentIndex() != EquipmentIndex.DroneBackup)
                    {
                        component3.CopyEquipmentFrom(master.inventory);
                    }
                }
                */
                component3.CopyItemsFrom(master.inventory);
                checkConfig(component3);
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
                /*
                if (EquipItems.Value)
                {
                    if(master.inventory.GetEquipmentIndex() != EquipmentIndex.DroneBackup)
                    {
                        component3.CopyEquipmentFrom(master.inventory);
                    }
                }
                */
                component3.CopyItemsFrom(master.inventory);
                checkConfig(component3);
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

            queensGuard();
            spookyGhosts();
        }

        public static void checkConfig(Inventory inventory)
        {
            // Items that will never be used by the NPCs.
            inventory.ResetItem(ItemIndex.TreasureCache);
            inventory.ResetItem(ItemIndex.Feather);
            inventory.ResetItem(ItemIndex.Firework);
            inventory.ResetItem(ItemIndex.Talisman);
            inventory.ResetItem(ItemIndex.SprintArmor);
            inventory.ResetItem(ItemIndex.JumpBoost);
            inventory.ResetItem(ItemIndex.GoldOnHit);
            inventory.ResetItem(ItemIndex.WardOnLevel);
            inventory.ResetItem(ItemIndex.BeetleGland);
            inventory.ResetItem(ItemIndex.CrippleWardOnLevel);

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

            if(ItemMultiplier.Value > 1)
            {
                int count = 0;
                if (Tier1Items.Value)
                {
                    foreach (ItemIndex index in ItemCatalog.tier1ItemList)
                    {
                        count = inventory.GetItemCount(index);
                        inventory.ResetItem(index);
                        inventory.GiveItem(index, (count * ItemMultiplier.Value));
                    }
                }
                if (Tier2Items.Value)
                {
                    foreach (ItemIndex index in ItemCatalog.tier2ItemList)
                    {
                        count = inventory.GetItemCount(index);
                        inventory.ResetItem(index);
                        inventory.GiveItem(index, (count * ItemMultiplier.Value));
                    }
                }
                if (Tier3Items.Value)
                {
                    foreach (ItemIndex index in ItemCatalog.tier3ItemList)
                    {
                        count = inventory.GetItemCount(index);
                        inventory.ResetItem(index);
                        inventory.GiveItem(index, (count * ItemMultiplier.Value));
                    }
                }
                if (LunarItems.Value)
                {
                    foreach (ItemIndex index in ItemCatalog.lunarItemList)
                    {
                        count = inventory.GetItemCount(index);
                        inventory.ResetItem(index);
                        inventory.GiveItem(index, (count * ItemMultiplier.Value));
                    }
                }               
            }
            
            /* Maybe when floats work in the configwrapper.
            else if (ItemMultiplier.Value < 1f)
            {
                int count = 0;
                foreach (ItemIndex index in ItemCatalog.tier1ItemList)
                {
                    count = inventory.GetItemCount(index);
                    inventory.ResetItem(index);
                    inventory.GiveItem(index, (int)Math.Ceiling(count * ItemMultiplier.Value));
                }
                foreach (ItemIndex index in ItemCatalog.tier2ItemList)
                {
                    count = inventory.GetItemCount(index);
                    inventory.ResetItem(index);
                    inventory.GiveItem(index, (int)Math.Ceiling(count * ItemMultiplier.Value));
                }
                foreach (ItemIndex index in ItemCatalog.tier3ItemList)
                {
                    count = inventory.GetItemCount(index);
                    inventory.ResetItem(index);
                    inventory.GiveItem(index, (int)Math.Ceiling(count * ItemMultiplier.Value));
                }
                foreach (ItemIndex index in ItemCatalog.lunarItemList)
                {
                    count = inventory.GetItemCount(index);
                    inventory.ResetItem(index);
                    inventory.GiveItem(index, (int)Math.Ceiling(count * ItemMultiplier.Value));
                }
            }
            */
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
                                    checkConfig(inventory);

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
                        component.inventory.CopyEquipmentFrom(inventory);
               
                    }
                    checkConfig(component.inventory);
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
    }
}
