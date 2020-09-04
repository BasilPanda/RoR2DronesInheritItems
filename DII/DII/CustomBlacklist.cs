using System;
using RoR2;

namespace Basil_ror2
{
    class CustomBlacklist
    {
        public static void customBlacklistChecker(CharacterMaster cm)
        {
            if (DII.CustomBlacklistEffectAll.Value)
            {
                customItem(cm);
                customEquip(cm);
                customItemCap(cm);
            }
            else
            {
                //Debug.Log(cm.name);
                Blacklist blacklist = new Blacklist();
                foreach (BlacklistProperties blp in blacklist.getList())
                {
                    if (cm.name == blp.id)
                    {
                        if (blp.EquipBlacklist != "")
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
            string[] customEquiplist = DII.CustomEquipBlacklistAll.Value.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

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
            string[] customItemlist = DII.CustomItemBlacklistAll.Value.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

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
            string[] customItemCaps = DII.CustomItemCapsAll.Value.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
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
    }
}
