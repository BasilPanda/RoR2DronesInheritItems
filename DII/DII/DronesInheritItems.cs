using System;
using BepInEx;
using BepInEx.Configuration;
using RoR2;

namespace Basil_ror2
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.Basil.DronesInheritItems", "DronesInheritItems", "2.4.7")]
    public class DII : BaseUnityPlugin
    {
        public static ConfigEntry<string> ItemMultiplier;
        public static ConfigEntry<bool> ItemRandomizer;
        public static ConfigEntry<bool> ItemGenerator;
        public static ConfigEntry<bool> UpdateInventory;

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

        public static ConfigEntry<bool> FixBackupDio;
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

        public static ConfigEntry<string> CustomItemBlacklist;
        public static ConfigEntry<string> CustomEquipBlacklist;
        public static ConfigEntry<string> CustomItemCaps;

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
            ItemIndex.TPHealingNova
        };

        public void InitConfig()
        {
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

            FixBackupDio = Config.Bind(
                "General Settings",
                "FixBackupDio",
                true,
                 "Makes it so that Backup drones will reinherit the 25 second death timer upon Dio revive."
                 );

            QueenGuardInherit = Config.Bind(
                "General Settings",
                "QueenGuardInherit",
                false,
                "Toggles Queen Guards to inherit/generate items."
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

            CustomItemBlacklist = Config.Bind(
                "General Settings",
                "CustomItemBlacklist",
                "",
                "Enter items ids separated by a comma and a space to blacklist inheritance/generation on certain items. ex) 41, 23, 17 \nItem ids: https://github.com/risk-of-thunder/R2Wiki/wiki/Item-&-Equipment-IDs-and-Names"
                );

            CustomEquipBlacklist = Config.Bind(
               "General Settings",
               "CustomEquipBlacklist",
               "",
               "Enter equipment ids separated by a comma and a space to blacklist inheritance/generation on certain equips. ex) 1, 14, 13 \nEquip ids: https://github.com/risk-of-thunder/R2Wiki/wiki/Item-&-Equipment-IDs-and-Names"
               );

            CustomItemCaps = Config.Bind(
               "General Settings",
               "CustomItemCaps",
               "",
               "Enter item ids as X-Y separated by a comma and a space to apply caps to certain items. X is the item id and Y is the number cap. ex) 0-20, 1-5, 2-1"
               );

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
            // DO LATER
            // Hooks.squidInherit();
            Chat.AddMessage("DronesInheritItems v2.4.7 Loaded!");
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
            foreach (ItemIndex item in ItemsNeverUsed)
            {
                inventory.ResetItem(item);
            }

            customItem(inventory);
            customEquip(inventory);
            customItemCap(inventory);
            
        }

        public static void customEquip(Inventory inventory)
        {
            // Custom Equip Blacklist
            string[] customEquiplist = CustomEquipBlacklist.Value.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string equip in customEquiplist)
            {
                if (Int32.TryParse(equip, out int x))
                {
                    if (inventory.GetEquipmentIndex() == (EquipmentIndex)x)
                    {
                        inventory.SetEquipmentIndex(EquipmentIndex.None);
                    }
                }
            }
        }

        public static void customItem(Inventory inventory)
        {
            // Custom Items Blacklist
            string[] customItemlist = CustomItemBlacklist.Value.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string item in customItemlist)
            {
                if (Int32.TryParse(item, out int x))
                {
                    inventory.ResetItem((ItemIndex)x);
                }
            }
        }

        public static void customItemCap(Inventory inventory)
        {
            // Custom item caps
            string [] customItemCaps = CustomItemCaps.Value.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach(string item in customItemCaps)
            {
                string[] temp = item.Split(new[] { '-' });
                if(temp.Length == 2)
                {
                    if (Int32.TryParse(temp[0], out int itemId) && Int32.TryParse(temp[1], out int cap))
                    {
                        if(inventory.GetItemCount((ItemIndex)itemId) > cap)
                        {
                            inventory.ResetItem((ItemIndex)itemId);
                            inventory.GiveItem((ItemIndex)itemId, cap);
                        }
                    }
                }
            }
        }

    }
}
