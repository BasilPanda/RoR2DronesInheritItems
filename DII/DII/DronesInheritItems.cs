﻿using BepInEx;
using BepInEx.Configuration;
using System.Collections.Generic;
using UnityEngine;
using RoR2;
using System;

namespace Basil_ror2
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.Basil.DronesInheritItems", "DronesInheritItems", "2.4.14")]
    public class DII : BaseUnityPlugin
    {
        #region General Config Wrappers

        public static ConfigEntry<string> ItemMultiplier;
        public static ConfigEntry<bool> ItemRandomizer;
        public static ConfigEntry<bool> ItemGenerator;
        public static ConfigEntry<bool> UpdateInventory;

        #endregion

        #region Generator Config Wrappers

        public static ConfigEntry<string> Tier1GenCap;
        public static ConfigEntry<string> Tier2GenCap;
        public static ConfigEntry<string> Tier3GenCap;
        public static ConfigEntry<string> LunarGenCap;
        public static ConfigEntry<string> Tier1GenChance;
        public static ConfigEntry<string> Tier2GenChance;
        public static ConfigEntry<string> Tier3GenChance;
        public static ConfigEntry<string> LunarGenChance;
        public static ConfigEntry<string> EquipGenChance;
        public static ConfigEntry<bool> LunarEquips;

        public static ConfigEntry<bool> Tier1Items;
        public static ConfigEntry<bool> Tier2Items;
        public static ConfigEntry<bool> Tier3Items;
        public static ConfigEntry<bool> LunarItems;
        public static ConfigEntry<bool> EquipItems;

        public static ConfigEntry<bool> InheritDio;
        public static ConfigEntry<bool> InheritHappiestMask;

        // Unintended features
        public static ConfigEntry<bool> FixBackupDio;
        public static ConfigEntry<string> BackupDeathTimer;
        public static ConfigEntry<bool> FixSquid;
        public static ConfigEntry<string> SquidHealthDecay;

        #endregion

        #region Base Inherit Config Wrappers

        public static ConfigEntry<bool> GunnerDronesInherit;
        public static ConfigEntry<bool> HealDronesInherit;
        public static ConfigEntry<bool> MissileDronesInherit;
        public static ConfigEntry<bool> FlameDronesInherit;
        public static ConfigEntry<bool> ProtoDronesInherit;
        public static ConfigEntry<bool> EquipDronesInherit;
        public static ConfigEntry<bool> EmergencyDronesInherit;
        public static ConfigEntry<bool> MinigunTurretsInherit;
        public static ConfigEntry<bool> SquidTurretsInherit;
        public static ConfigEntry<bool> BackupDronesInherit;
        public static ConfigEntry<bool> QueenGuardInherit;
        public static ConfigEntry<bool> GhostInherit;
        public static ConfigEntry<bool> GoldTitanInherit;

        #endregion

        #region Blacklist Config Wrappers
        // Blacklist settings
        public static ConfigEntry<bool> CustomBlacklistEffectAll;
        public static ConfigEntry<string> CustomItemBlacklistAll;
        public static ConfigEntry<string> CustomEquipBlacklistAll;
        public static ConfigEntry<string> CustomItemCapsAll;

        public static ConfigEntry<string> CBItemMinigun;
        public static ConfigEntry<string> CBItemCapMinigun;

        public static ConfigEntry<string> CBItemGunnerDrone;
        public static ConfigEntry<string> CBItemCapGunnerDrone;

        public static ConfigEntry<string> CBItemHealDrone;
        public static ConfigEntry<string> CBItemCapHealDrone;

        public static ConfigEntry<string> CBItemMegaDrone;
        public static ConfigEntry<string> CBItemCapMegaDrone;

        public static ConfigEntry<string> CBItemMissileDrone;
        public static ConfigEntry<string> CBItemCapMissileDrone;

        public static ConfigEntry<string> CBItemFlameDrone;
        public static ConfigEntry<string> CBItemCapFlameDrone;

        public static ConfigEntry<string> CBItemBackup;
        public static ConfigEntry<string> CBItemCapBackup;

        public static ConfigEntry<string> CBItemEmergencyDrone;
        public static ConfigEntry<string> CBItemCapEmergencyDrone;

        public static ConfigEntry<string> CBItemEquipDrone;
        public static ConfigEntry<string> CBEquipEquipDrone;
        public static ConfigEntry<string> CBItemCapEquipDrone;

        public static ConfigEntry<string> CBItemSquid;
        public static ConfigEntry<string> CBItemCapSquid;

        public static ConfigEntry<string> CBItemQueensGuard;
        public static ConfigEntry<string> CBEquipQueensGuard;
        public static ConfigEntry<string> CBItemCapQueensGuard;

        public static ConfigEntry<string> CBItemTitan;
        public static ConfigEntry<string> CBEquipTitan;
        public static ConfigEntry<string> CBItemCapTitan;

        public static ConfigEntry<string> CBItemGhosts;
        public static ConfigEntry<string> CBEquipGhosts;
        public static ConfigEntry<string> CBItemCapGhosts;
        #endregion 

        public static EquipmentIndex[] LunarEquipmentList = new EquipmentIndex[]
        {
            EquipmentIndex.Meteor,
            EquipmentIndex.LunarPotion, // no idea what this is but it has lunar on it :D
            EquipmentIndex.BurnNearby,
            EquipmentIndex.CrippleWard
        };

        public static ItemIndex[] ItemsNeverUsed = new ItemIndex[]
        {
            ItemIndex.SprintWisp,
            ItemIndex.TitanGoldDuringTP,
            ItemIndex.TreasureCache,
            ItemIndex.Feather,
            ItemIndex.Firework,
            ItemIndex.SprintArmor,
            ItemIndex.JumpBoost,
            ItemIndex.GoldOnHit,
            ItemIndex.WardOnLevel,
            ItemIndex.BeetleGland,
            ItemIndex.CrippleWardOnLevel,
            ItemIndex.TPHealingNova,
            ItemIndex.FocusConvergence
        };

        public void InitConfig()
        {
            #region Base Inherit Settings

            GunnerDronesInherit = Config.Bind(
                "Base Inherit Settings",
                "GunnerDronesInherit",
                true,
                "Toggles Gunner drones to inherit items."
                );

            HealDronesInherit = Config.Bind(
                "Base Inherit Settings",
                "HealDronesInherit",
                true,
                "Toggles Healing drones to inherit items."
                );

            MissileDronesInherit = Config.Bind(
                "Base Inherit Settings",
                "MissileDronesInherit",
                true,
                "Toggles Missile drones to inherit items."
                );

            FlameDronesInherit = Config.Bind(
                "Base Inherit Settings",
                "FlameDronesInherit",
                true,
                "Toggles Incinierator drones to inherit items."
                );

            ProtoDronesInherit = Config.Bind(
                "Base Inherit Settings",
                "ProtoDronesInherit",
                true,
                "Toggles TC-280 Prototype drones to inherit items."
                );

            EquipDronesInherit = Config.Bind(
                "Base Inherit Settings",
                "EquipDronesInherit",
                true,
                "Toggles Equipment drones to inherit items."
                );

            EmergencyDronesInherit = Config.Bind(
                "Base Inherit Settings",
                "EmergencyDronesInherit",
                true,
                "Toggles Emergency drones to inherit items."
                );

            MinigunTurretsInherit = Config.Bind(
                "Base Inherit Settings",
                "MinigunTurretsInherit",
                true,
                "Toggles ONLY purchasable minigun turrets to inherit items."
                );

            SquidTurretsInherit = Config.Bind(
                "Base Inherit Settings",
                "SquidTurretsInherit",
                true,
                "Toggles ONLY squid turrets to inherit items."
                );

            BackupDronesInherit = Config.Bind(
                "Base Inherit Settings",
                "BackupDronesInherit",
                true,
                "Toggles Backup Drones to inherit items.");

            ItemMultiplier = Config.Bind(
                "Base Inherit Settings",
                "ItemMultiplier",
                "1",
                "Sets the multiplier for items to be multiplied by when the drones inherit them."
                );

            ItemRandomizer = Config.Bind(
                "Base Inherit Settings",
                "ItemRandomizer",
                false,
                "Toggles random amounts of items to be inherited from the player's inventory."
                );

            UpdateInventory = Config.Bind(
                "Base Inherit Settings",
                "UpdateInventory",
                false,
                "Toggles updating drone inventory after every stage completion."
                );

            #endregion Base Inherit Settings END

            #region Generator Settings

            ItemGenerator = Config.Bind(
                "Generator Settings",
                "ItemGenerator",
                false,
                "Toggles generation of items instead of inheriting items from player's inventory. WILL IGNORE BASE INHERIT SETTINGS IF TRUE."
                );

            Tier1GenCap = Config.Bind(
                "Generator Settings",
                "Tier1GenCap",
                "4",
                "The multiplicative max item cap for generating Tier 1 (white) items."
                );

            Tier2GenCap = Config.Bind(
                "Generator Settings",
                "Tier2GenCap",
                "2",
                "The multiplicative max item cap for generating Tier 2 (green) items."
                );

            Tier3GenCap = Config.Bind(
                "Generator Settings",
                "Tier3GenCap",
                "1",
                "The multiplicative max item cap for generating Tier 3 (red) items."
                );

            LunarGenCap = Config.Bind(
                "Generator Settings",
                "LunarGenCap",
                "1",
                "The multiplicative max item cap for generating Lunar (blue) items."
                );

            Tier1GenChance = Config.Bind(
                "Generator Settings",
                "Tier1GenChance",
                "40",
                "The percent chance for generating a Tier 1 (white) item."
                );

            Tier2GenChance = Config.Bind(
                "Generator Settings",
                "Tier2GenChance",
                "20",
                "The percent chance for generating a Tier 2 (green) item."
                );

            Tier3GenChance = Config.Bind(
                "Generator Settings",
                "Tier3GenChance",
                "1",
                "The percent chance for generating a Tier 3 (red) item."
                );

            LunarGenChance = Config.Bind(
                "Generator Settings",
                "LunarGenChance",
                "0.5",
                "The percent chance for generating a Lunar (blue) item."
                );

            EquipGenChance = Config.Bind(
                "Generator Settings",
                "EquipGenChance",
                "10",
                "The percent chance for generating a Use item."
                );
            #endregion Generator Settings END

            #region General Settings

            Tier1Items = Config.Bind(
                "General Settings",
                "Tier1Items",
                true,
                "Toggles Tier 1 (white) items to be inherited/generated."
                );

            Tier2Items = Config.Bind(
                "General Settings",
                "Tier2Items",
                true,
                "Toggles Tier 2 (green) items to be inherited/generated."
                );

            Tier3Items = Config.Bind(
                "General Settings",
                "Tier3Items",
                true,
                "Toggles Tier 3 (red) items to be inherited/generated."
                );

            LunarItems = Config.Bind(
                "General Settings",
                "LunarItems",
                true,
                "Toggles Lunar (blue) items to be inherited/generated."
                );

            EquipItems = Config.Bind(
                "General Settings",
                "EquipItems",
                false,
                "Toggles Use items to be inherited/generated. ONLY WORKS FOR AURELIONITE, QUEEN'S GUARD AND GHOSTS FROM HAPPIEST MASK."
                );

            LunarEquips = Config.Bind(
                "General Settings",
                "LunarEquips",
                false,
                "Toggles Lunar Use items to be inherited/generated. ONLY WORKS FOR AURELIONITE, QUEEN'S GUARD AND GHOSTS FROM HAPPIEST MASK."
                );

            InheritDio = Config.Bind(
                "General Settings",
                "InheritDio",
                true,
                "Toggles Dio's Best Friend to be inherited/generated by ALL types."
                );

            InheritHappiestMask = Config.Bind(
                "General Settings",
                "InheritHappiestMask",
                true,
                "Toggles Happiest Mask to be inherited/generated by ALL types."
                );

            GhostInherit = Config.Bind(
                "General Settings",
                "GhostInherit",
                false,
                "Toggles ghosts spawned from Happiest Mask to inherit/generate items.\n" +
                "Dev notice: If ghosts create other ghosts, damage for the new ghost will\nbe multiplied " +
                "by 500% ON TOP of the original ghost 500% damage buff.\nThis cycle can continue non stop."
                );

            GoldTitanInherit = Config.Bind(
                "General Settings",
                "GoldTitanInherit",
                false,
                "Toggles allied Aurelionite from Halcyon Seed to inherit/generate items."
                );

            QueenGuardInherit = Config.Bind(
                "General Settings",
                "QueenGuardInherit",
                false,
                "Toggles Queen Guards to inherit/generate items."
                );
            #endregion General Settings END

            #region Unintended Features Settings

            FixBackupDio = Config.Bind(
                "Unintended Mod Features Settings",
                "FixBackupDio",
                true,
                 "Makes it so that Backup drones will reinherit the BackupDeathTimer setting upon Dio revive."
                 );

            BackupDeathTimer = Config.Bind(
                "Unintended Mod Features Settings",
                "BackupDeathTimer",
                "25",
                "The seconds it takes for Backup Drones to expire."
                );

            FixSquid = Config.Bind(
                "Unintended Mod Features Settings",
                "FixSquid",
                true,
                 "Makes it so that squids will die normally over time using the SquidHealthDecay setting."
                 );

            SquidHealthDecay = Config.Bind(
                "Unintended Mod Features Settings",
                "SquidHealthDecay",
                "40",
                "The rate at which a squid turret's health would decay"
                );

            #endregion Unintended Features Settings END

            #region Blacklist Settings

            #region Effect All Settings
            CustomBlacklistEffectAll = Config.Bind(
                "Blacklist Settings",
                "CustomBlacklistEffectAll",
                true,
                "Toggles usage of blacklist affecting all allies. True uses the settings ending with All."
                );

            CustomItemBlacklistAll = Config.Bind(
                "Blacklist Settings",
                "CustomItemBlacklistAll",
                "",
                "Enter items ids separated by a comma and a space to blacklist inheritance/generation on certain items. ex) 41, 23, 17 \nItem ids: https://github.com/risk-of-thunder/R2Wiki/wiki/Item-&-Equipment-IDs-and-Names"
                );

            CustomEquipBlacklistAll = Config.Bind(
                "Blacklist Settings",
                "CustomEquipBlacklistAll",
                "",
                "Enter equipment ids separated by a comma and a space to blacklist inheritance/generation on certain equips. ex) 1, 14, 13 \nEquip ids: https://github.com/risk-of-thunder/R2Wiki/wiki/Item-&-Equipment-IDs-and-Names"
                );

            CustomItemCapsAll = Config.Bind(
                "Blacklist Settings",
                "CustomItemCapsAll",
                "",
                "Enter item ids as X-Y separated by a comma and a space to apply caps to certain items. X is the item id and Y is the number cap. ex) 0-20, 1-5, 2-1"
                );

            #endregion All Settings

            #region Squid

            CBItemSquid = Config.Bind(
                "Blacklist Settings",
                "CBItemSquid",
                "",
                "Blacklist items targeting squid turrets. Enter item ids the same way as CustomItemBlacklistAll."
                );

            CBItemCapSquid = Config.Bind(
                "Blacklist Settings",
                "CBItemCapSquid",
                "",
                "Cap items for squid turrets. Enter ids the same way as CustomItemCapsAll."
                );

            #endregion Squid

            #region Minigun Turret

            CBItemMinigun = Config.Bind(
                "Blacklist Settings",
                "CBItemMinigun",
                "",
                "Blacklist items targeting minigun turrets. Enter item ids the same way as CustomItemBlacklistAll."
                );

            CBItemCapMinigun = Config.Bind(
                "Blacklist Settings",
                "CBItemCapMinigun",
                "",
                "Cap items for minigun turrets. Enter ids the same way as CustomItemCapsAll."
                );

            #endregion Minigun Turret

            #region Gunner Drone

            CBItemGunnerDrone = Config.Bind(
                "Blacklist Settings",
                "CBItemGunnerDrone",
                "",
                "Blacklist items targeting gunner drones. Enter item ids the same way as CustomItemBlacklistAll."
                );

            CBItemCapGunnerDrone = Config.Bind(
                "Blacklist Settings",
                "CBItemCapGunnerDrone",
                "",
                "Cap items for gunner drones. Enter ids the same way as CustomItemCapsAll."
                );

            #endregion Gunner Drone

            #region Heal Drone

            CBItemHealDrone = Config.Bind(
                "Blacklist Settings",
                "CBItemHealDrone",
                "",
                "Blacklist items targeting heal drones. Enter item ids the same way as CustomItemBlacklistAll."
                );

            CBItemCapHealDrone = Config.Bind(
                "Blacklist Settings",
                "CBItemCapHealDrone",
                "",
                "Cap items for heal drones. Enter ids the same way as CustomItemCapsAll."
                );

            #endregion Heal Drone

            #region Mega Drone

            CBItemMegaDrone = Config.Bind(
                "Blacklist Settings",
                "CBItemMegaDrone",
                "",
                "Blacklist items targeting TC-280 drones. Enter item ids the same way as CustomItemBlacklistAll."
                );

            CBItemCapMegaDrone = Config.Bind(
                "Blacklist Settings",
                "CBItemCapMegaDrone",
                "",
                "Cap items for TC-280 drones. Enter ids the same way as CustomItemCapsAll."
                );

            #endregion Mega Drone

            #region Missile Drone

            CBItemMissileDrone = Config.Bind(
                "Blacklist Settings",
                "CBItemMissileDrone",
                "",
                "Blacklist items targeting missile drones. Enter item ids the same way as CustomItemBlacklistAll."
                );

            CBItemCapMissileDrone = Config.Bind(
                "Blacklist Settings",
                "CBItemCapMissileDrone",
                "",
                "Cap items for missile drones. Enter ids the same way as CustomItemCapsAll."
                );

            #endregion Missile Drone

            #region FlameDrone

            CBItemFlameDrone = Config.Bind(
                "Blacklist Settings",
                "CBItemFlameDrone",
                "",
                "Blacklist items targeting flame drones. Enter item ids the same way as CustomItemBlacklistAll."
                );

            CBItemCapFlameDrone = Config.Bind(
                "Blacklist Settings",
                "CBItemCapFlameDrone",
                "",
                "Cap items for flame drones. Enter ids the same way as CustomItemCapsAll."
                );

            #endregion Flame Drone

            #region Backup Drone

            CBItemBackup = Config.Bind(
                "Blacklist Settings",
                "CBItemBackup",
                "",
                "Blacklist items targeting The Backup drones. Enter item ids the same way as CustomItemBlacklistAll."
                );

            CBItemCapBackup = Config.Bind(
                "Blacklist Settings",
                "CBItemCapBackup",
                "",
                "Cap items for The Backup drones. Enter ids the same way as CustomItemCapsAll."
                );

            #endregion Flame Drone

            #region EmergencyDrone

            CBItemEmergencyDrone = Config.Bind(
                "Blacklist Settings",
                "CBItemEmergencyDrone",
                "",
                "Blacklist items targeting emergency drones. Enter item ids the same way as CustomItemBlacklistAll."
                );

            CBItemCapEmergencyDrone = Config.Bind(
                "Blacklist Settings",
                "CBItemCapEmergencyDrone",
                "",
                "Cap items for emergency drones. Enter ids the same way as CustomItemCapsAll."
                );

            #endregion Emergency Drone

            #region Equip Drone
            CBItemEquipDrone = Config.Bind(
                "Blacklist Settings",
                "CBItemEquipDrone",
                "",
                "Blacklist items targeting equipment drones. Enter item ids the same way as CustomItemBlacklistAll."
                );

            CBEquipEquipDrone = Config.Bind(
                "Blacklist Settings",
                "CBEquipEquipDrone",
                "",
                "Blacklist equips targeting equipment drones. Enter equip ids the same way as CustomEquipBlacklistAll."
                );

            CBItemCapEquipDrone = Config.Bind(
                "Blacklist Settings",
                "CBItemCapEquipDrone",
                "",
                "Cap items for equipment drones. Enter ids the same way as CustomItemCapsAll."
                );

            #endregion Equip Drone

            #region QueensGuard

            CBItemQueensGuard = Config.Bind(
                "Blacklist Settings",
                "CBItemQueensGuard",
                "",
                "Blacklist items targeting queen guards. Enter item ids the same way as CustomItemBlacklistAll."
                );

            CBEquipQueensGuard = Config.Bind(
                "Blacklist Settings",
                "CBEquipQueensGuard",
                "",
                "Blacklist equips targeting queen guards. Enter equip ids the same way as CustomEquipBlacklistAll."
                );

            CBItemCapQueensGuard = Config.Bind(
                "Blacklist Settings",
                "CBItemCapQueensGuard",
                "",
                "Cap items for queen guards. Enter ids the same way as CustomItemCapsAll."
                );

            #endregion QueensGuard

            #region Titan

            CBItemTitan = Config.Bind(
                "Blacklist Settings",
                "CBItemTitan",
                "",
                "Blacklist items targeting Aurelionite. Enter item ids the same way as CustomItemBlacklistAll."
                );

            CBEquipTitan = Config.Bind(
                "Blacklist Settings",
                "CBEquipTitan",
                "",
                "Blacklist equips targeting Aurelionite. Enter equip ids the same way as CustomEquipBlacklistAll."
                );

            CBItemCapTitan = Config.Bind(
                "Blacklist Settings",
                "CBItemCapTitan",
                "",
                "Cap items for Aurelionite. Enter ids the same way as CustomItemCapsAll."
                );

            #endregion Titan

            #region Ghosts

            CBItemGhosts = Config.Bind(
                "Blacklist Settings",
                "CBItemGhosts",
                "",
                "Blacklist items targeting Ghosts. Enter item ids the same way as CustomItemBlacklistAll."
                );

            CBEquipGhosts = Config.Bind(
                "Blacklist Settings",
                "CBEquipGhosts",
                "",
                "Blacklist equips targeting Ghosts. Enter equip ids the same way as CustomEquipBlacklistAll."
                );

            CBItemCapGhosts = Config.Bind(
                "Blacklist Settings",
                "CBItemCapGhosts",
                "",
                "Cap items for Ghosts. Enter ids the same way as CustomItemCapsAll."
                );

            #endregion Ghosts

            #endregion Blacklist Settings END

        }

        public static float ConfigToFloat(string configline)
        {
            if (float.TryParse(configline, out float x))
            {
                return x;
            }
            return 1f;
        }

        public void Awake()
        {
            InitConfig();
            Hooks.fixBackupDio();
            Hooks.spookyGhosts();
            Hooks.titanGold();
            Hooks.backupDrones();
            Hooks.queensGuard();
            Hooks.baseMod();
            Hooks.updateAfterStage();
            Chat.AddMessage("DronesInheritItems v2.4.14 Loaded!");
        }

        public static void checkConfig(CharacterMaster cm, CharacterMaster master)
        {
            Inventory inventory = cm.inventory;
            if (ItemGenerator.Value) // Using generator instead
            {
                resetInventory(inventory);
                int scc = Run.instance.stageClearCount;
                if (Tier1Items.Value)
                {
                    foreach (ItemIndex index in ItemCatalog.tier1ItemList)
                    {
                        if (Util.CheckRoll(ConfigToFloat(Tier1GenChance.Value), master))
                        {
                            inventory.GiveItem(index, UnityEngine.Random.Range(0, (int)(scc * ConfigToFloat(Tier1GenCap.Value) + 1)));
                        }
                    }
                }
                if (Tier2Items.Value)
                {
                    foreach (ItemIndex index in ItemCatalog.tier2ItemList)
                    {
                        if (Util.CheckRoll(ConfigToFloat(Tier2GenChance.Value), master))
                        {
                            inventory.GiveItem(index, UnityEngine.Random.Range(0, (int)(scc * ConfigToFloat(Tier2GenCap.Value) + 1)));
                        }
                    }
                }
                if (Tier3Items.Value)
                {
                    foreach (ItemIndex index in ItemCatalog.tier3ItemList)
                    {
                        if (Util.CheckRoll(ConfigToFloat(Tier3GenChance.Value), master))
                        {
                            inventory.GiveItem(index, UnityEngine.Random.Range(0, (int)(scc * ConfigToFloat(Tier3GenCap.Value) + 1)));
                        }
                    }
                }
                if (LunarItems.Value)
                {
                    foreach (ItemIndex index in ItemCatalog.lunarItemList)
                    {
                        if (Util.CheckRoll(ConfigToFloat(LunarGenChance.Value), master))
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
                    if (Util.CheckRoll(ConfigToFloat(EquipGenChance.Value), master))
                    {
                        inventory.ResetItem(ItemIndex.AutoCastEquipment);
                        inventory.GiveItem(ItemIndex.AutoCastEquipment, 1);
                        //EquipmentIndex equipmentIndex = Run.instance.availableEquipmentDropList[Run.instance.spawnRng.RangeInt(0, Run.instance.availableEquipmentDropList.Count)].equipmentIndex;
                        EquipmentIndex equipmentIndex = EquipmentCatalog.equipmentList[Run.instance.spawnRng.RangeInt(0, Run.instance.availableEquipmentDropList.Count)];
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
                
                // Items never used.
                foreach (ItemIndex item in ItemsNeverUsed)
                {
                    inventory.ResetItem(item);
                }

            }
            else // Default inheritance
            {
                updateInventory(cm, master);
            }
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

        public static void updateInventory(CharacterMaster cm, CharacterMaster master)
        {
            Inventory inventory = cm.inventory;
            inventory.CopyItemsFrom(master.inventory);
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
                            cm.inventory.SetEquipmentIndex(EquipmentIndex.Fruit); // default to fruit
                            break;
                        }
                    }
                }
            }

            // Items that will never be used by the NPCs.
            foreach (ItemIndex item in ItemsNeverUsed)
            {
                cm.inventory.ResetItem(item);
            }

            customBlacklistChecker(cm);
        }

        public static void customBlacklistChecker(CharacterMaster cm)
        {
            if(CustomBlacklistEffectAll.Value)
            {
                customItem(cm);
                customEquip(cm);
                customItemCap(cm);
            } else
            {
                //Debug.Log(cm.name);
                Blacklist blacklist = new Blacklist();
                foreach (BlacklistProperties blp in blacklist.getList())
                {
                    if(cm.name == blp.id)
                    {
                        if(blp.EquipBlacklist != "")
                        {
                            customEquip(cm, blp.EquipBlacklist);
                        }
                        customItem(cm, blp.ItemBlacklist);
                        customItem(cm, blp.ItemCaps);
                    }
                }
            }
        }

        public static void customEquip(CharacterMaster cm, string customEquipList)
        {
            // Custom Equip Blacklist
            string[] customEquiplist = customEquipList.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string equip in customEquiplist)
            {
                if (Int32.TryParse(equip, out int x))
                {
                    if (cm.inventory.GetEquipmentIndex() == (EquipmentIndex)x)
                    {
                        cm.inventory.SetEquipmentIndex(EquipmentIndex.None);
                    }
                }
            }
        }

        public static void customEquip(CharacterMaster cm)
        {
            // Custom Equip Blacklist
            string[] customEquiplist = CustomEquipBlacklistAll.Value.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string equip in customEquiplist)
            {
                if (Int32.TryParse(equip, out int x))
                {
                    if (cm.inventory.GetEquipmentIndex() == (EquipmentIndex)x)
                    {
                        cm.inventory.SetEquipmentIndex(EquipmentIndex.None);
                    }
                }
            }
        }

        public static void customItem(CharacterMaster cm, string customItemList)
        {
            // Custom Items Blacklist
            string[] customItemlist = customItemList.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string item in customItemlist)
            {
                if (Int32.TryParse(item, out int x))
                {
                    cm.inventory.ResetItem((ItemIndex)x);
                }
            }
        }

        public static void customItem(CharacterMaster cm)
        {
            // Custom Items Blacklist
            string[] customItemlist = CustomItemBlacklistAll.Value.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string item in customItemlist)
            {
                if (Int32.TryParse(item, out int x))
                {
                    cm.inventory.ResetItem((ItemIndex)x);
                }
            }
        }

        public static void customItemCap(CharacterMaster cm, string customCaps)
        {
            // Custom item caps
            string[] customItemCaps = customCaps.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string item in customItemCaps)
            {
                string[] temp = item.Split(new[] { '-' });
                if (temp.Length == 2)
                {
                    if (Int32.TryParse(temp[0], out int itemId) && Int32.TryParse(temp[1], out int cap))
                    {
                        if (cm.inventory.GetItemCount((ItemIndex)itemId) > cap)
                        {
                            cm.inventory.ResetItem((ItemIndex)itemId);
                            cm.inventory.GiveItem((ItemIndex)itemId, cap);
                        }
                    }
                }
            }
        }

        public static void customItemCap(CharacterMaster cm)
        {
            // Custom item caps
            string [] customItemCaps = CustomItemCapsAll.Value.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach(string item in customItemCaps)
            {
                string[] temp = item.Split(new[] { '-' });
                if(temp.Length == 2)
                {
                    if (Int32.TryParse(temp[0], out int itemId) && Int32.TryParse(temp[1], out int cap))
                    {
                        if(cm.inventory.GetItemCount((ItemIndex)itemId) > cap)
                        {
                            cm.inventory.ResetItem((ItemIndex)itemId);
                            cm.inventory.GiveItem((ItemIndex)itemId, cap);
                        }
                    }
                }
            }
        }

    }
}
