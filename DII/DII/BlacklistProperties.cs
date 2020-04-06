using System;
using System.Collections.Generic;
using System.Text;

namespace Basil_ror2
{

    public class BlacklistProperties
    {
        public string id;
        public string ItemBlacklist;
        public string EquipBlacklist;
        public string ItemCaps;

        public BlacklistProperties(string id, string ItemBlacklist, string EquipBlacklist, string ItemCaps)
        {
            this.id = id;
            this.ItemBlacklist = ItemBlacklist;
            this.EquipBlacklist = EquipBlacklist;
            this.ItemCaps = ItemCaps;
        }

        public BlacklistProperties(string id, string ItemBlacklist, string ItemCaps)
        {
            this.id = id;
            this.ItemBlacklist = ItemBlacklist;
            this.EquipBlacklist = null;
            this.ItemCaps = ItemCaps;
        }
    }
}
