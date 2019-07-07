using System;
using BepInEx;
using BepInEx.Configuration;
using RoR2;

namespace Basil_ror2
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.Basil.DronesInheritItems", "DronesInheritItems", "2.4.3")]
    public class DII : BaseUnityPlugin
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
        public static ConfigWrapper<bool> GunnerDronesInherit;
        public static ConfigWrapper<bool> HealDronesInherit;
        public static ConfigWrapper<bool> MissileDronesInherit;
        public static ConfigWrapper<bool> FlameDronesInherit;
        public static ConfigWrapper<bool> ProtoDronesInherit;
        public static ConfigWrapper<bool> EquipDronesInherit;
        public static ConfigWrapper<bool> TurretsInherit;
        public static ConfigWrapper<bool> BackupDronesInherit;
        public static ConfigWrapper<bool> QueenGuardInherit;
        public static ConfigWrapper<bool> GhostInherit;
        public static ConfigWrapper<bool> GoldTitanInherit;

        public static EquipmentIndex[] LunarEquipmentList = new EquipmentIndex[]
        {
            EquipmentIndex.Meteor,
            EquipmentIndex.LunarPotion, // no idea what this is but it has lunar on it :D
            EquipmentIndex.BurnNearby,
            EquipmentIndex.CrippleWard
        };
        
        public void InitConfig()
        {
            GunnerDronesInherit = Config.Wrap(
                "Base Inherit Settings",
                "GunnerDronesInherit",
                "Toggles Gunner drones to inherit items.",
                true);

            HealDronesInherit = Config.Wrap(
                "Base Inherit Settings",
                "HealDronesInherit",
                "Toggles Healing drones to inherit items.",
                true);

            MissileDronesInherit = Config.Wrap(
                "Base Inherit Settings",
                "MissileDronesInherit",
                "Toggles Missile drones to inherit items.",
                true);

            FlameDronesInherit = Config.Wrap(
                "Base Inherit Settings",
                "FlameDronesInherit",
                "Toggles Incinierator drones to inherit items.",
                true);

            ProtoDronesInherit = Config.Wrap(
                "Base Inherit Settings",
                "ProtoDronesInherit",
                "Toggles TC-280 Prototype drones to inherit items.",
                true);

            EquipDronesInherit = Config.Wrap(
                "Base Inherit Settings",
                "EquipDronesInherit",
                "Toggles Equipment drones to inherit items.",
                true);

            TurretsInherit = Config.Wrap(
                "Base Inherit Settings",
                "TurretsInherit",
                "Toggles ONLY purchasable turrets to inherit items.",
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
                "Toggles Use items to be inherited/generated. ONLY WORKS FOR AURELIONITE, QUEEN'S GUARD AND GHOSTS FROM HAPPIEST MASK.",
                false);

            LunarEquips = Config.Wrap(
                "General Settings",
                "LunarEquips",
                "Toggles Lunar Use items to be inherited/generated. ONLY WORKS FOR AURELIONITE, QUEEN'S GUARD AND GHOSTS FROM HAPPIEST MASK.",
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

            GoldTitanInherit = Config.Wrap(
                "General Settings",
                "GoldTitanInherit",
                "Toggles allied Aurelionite from Halcyon Seed to inherit/generate items.",
                false);

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
            Chat.AddMessage("DronesInheritItems v2.4.3 Loaded!");
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
            else // Default inheritance
            {
                updateInventory(inventory, master);
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


    }
}
