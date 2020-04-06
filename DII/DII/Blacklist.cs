using System;
using System.Collections.Generic;
using System.Text;

namespace Basil_ror2
{
    public class Blacklist
    {
        public static List<BlacklistProperties> blacklistProperties = new List<BlacklistProperties>()
        {
            new BlacklistProperties("SquidTurretMaster(Clone)", DII.CBItemSquid.Value, DII.CBItemCapSquid.Value),
            new BlacklistProperties("Turret1Master", DII.CBItemMinigun.Value, DII.CBItemCapMinigun.Value),
            new BlacklistProperties("Drone1Master", DII.CBItemGunnerDrone.Value, DII.CBItemCapGunnerDrone.Value),
            new BlacklistProperties("Drone2Master", DII.CBItemHealDrone.Value, DII.CBItemCapHealDrone.Value),
            new BlacklistProperties("DroneBackupMaster(Clone)", DII.CBItemBackup.Value, DII.CBItemCapBackup.Value),
            new BlacklistProperties("DroneMissileMaster", DII.CBItemMissileDrone.Value, DII.CBItemCapMissileDrone.Value),
            new BlacklistProperties("FlameDroneMaster", DII.CBItemFlameDrone.Value, DII.CBItemCapFlameDrone.Value),
            new BlacklistProperties("MegaDroneMaster", DII.CBItemMegaDrone.Value, DII.CBItemCapMegaDrone.Value),
            new BlacklistProperties("EquipmentDroneMaster", DII.CBItemEquipDrone.Value, DII.CBItemCapEquipDrone.Value),
            new BlacklistProperties("EmergencyDroneMaster", DII.CBItemEmergencyDrone.Value, DII.CBItemCapEmergencyDrone.Value),
            new BlacklistProperties("BeetleGuardAllyMaster(Clone)", DII.CBItemQueensGuard.Value, DII.CBEquipQueensGuard.Value, DII.CBItemCapQueensGuard.Value),
            new BlacklistProperties("titanGoldBossMaster", DII.CBItemTitan.Value, DII.CBEquipTitan.Value, DII.CBItemCapTitan.Value),
        };

        public List<BlacklistProperties> getList()
        {
            return blacklistProperties;
        }

        public override string ToString()
        {
            string s = "";
            foreach(BlacklistProperties blp in blacklistProperties)
            {
                s += blp.id;
            }
            return s;
        }
    }
}
