# Drones Inherit Items
## By Basil
A mod that allows **all** drones and turrets to inherit items from the player.

- **Has AURELIONITE, QUEEN GUARDS, GHOSTS, SQUID TURRETS, and lots of configuration!** 

- Can update inventories after each stage clear!

- Can generate items for drones with the ItemGenerator option in the config file! *Will ignore inherit settings.*

By default, only drones and turrets inherit items.

Please contact Basil#7379 on Discord for any issues besides the known bugs!

## Installation
1. Install [BepInEx Mod Pack](https://thunderstore.io/package/bbepis/BepInExPack/)
2. Install [R2API](https://thunderstore.io/package/tristanmcpherson/R2API/)
3. Download the latest DronesInheritItems.dll here.
4. Move DronesInheritItems.dll to your \BepInEx\plugins folder

## Configuration

1. To find the config file, first start up the game with DronesInheritItems.dll in your \BepInEx\plugins folder already!
2. Then go to \BepInEx\config and open com.Basil.DronesInheritItems.cfg

**I highly recommend deleting the config file if you previously installed this mod so it can be updated with the most recent config layout.**

## Ingame Examples

[Ghosts](https://www.youtube.com/watch?v=8OT75rt7Bro)

[Gunner Drones](https://www.youtube.com/watch?v=aDg-Q41yez8&feature=youtu.be) (also had 4x modded base drone attack speed)

[Back-up Drones](https://www.youtube.com/watch?v=vYXISaecv74&feature=youtu.be) (also had 4x modded base drone attack speed)

## FAQ

Q: Who does Aurelionite inherit items from?

A: To whoever owns the item. If there are multiple people who own the item, then it selects randomly.

Q: How do you calculate the item max cap for item generation?

A: Currently the way it is done is randomly selecting a value from 0 to the current stage * item max cap, inclusively, for every item.

Q: How does the ItemRandomizer setting work?

A: It will randomly choose a value between 0 and the player's item count for the respective item if the setting is true.

## Known Bugs

- None at the moment!

## Changelog

**v2.4.14**

- Updated for v1.0 of the game!
- Updated for 2.5.7 R2API.
- Permanently blacklisted Visions of Heresy on Emergency Drones.
- Fixed multiplayer latency issues. Thanks to @terribleperson#7284 for reporting it!

**v2.4.13**

- Ghosts should decay as intended now. Thanks to @Vashzaron#7672 for reporting it!

**v2.4.12**

- Squid Turrets should now inherit from the person who activated instead of randomly choosing a player in coop situations.
- Permanently blacklisted Visions of Heresy on Healing Drones.
- Added custom blacklist options for Ghosts.

**v2.4.11**

- Fixed the return of the squid turret preventing progression bug again to next stage when having update after every stage option on. Thanks to @Solacex#4276 for reporting it!

**v2.4.10**

- Fixed teleporter bubble not showing up bug. Thanks to @Vashzaron#7672 for reporting it!

**v2.4.9**

- Fixed squid turret preventing progression to next stage when having update after every stage option on. Thanks to @Daedreus#3431 for reporting it!

**v2.4.8**

- Updated for R2API v2.4.10
- Ghost inherits now have ghostly effect (forgot to put this in last update)
- Major configuration update allowing for custom blacklisting for specific allies.

**v2.4.7**

- Updated for the Artifacts update!
- Added squid inherit setting (default to true)

**v2.4.6**

- Updated for Hidden Realms update!
- Added Custom Item Caps config!

**v2.4.5**

- Updated for Skills 2.0 update!

**v2.4.4**

- Update inventory will no longer update drone inventories for drone types set to false
- Added custom item blacklist setting
- Added custom equipment blacklist setting

**v2.4.3**

- Updated R2API dependency string

**v2.4.2**

- Fixed no sound on enemy spawn bug 
- Allied Aurelionite from Halcyon Seed can now inherit items (default to false)

**v2.4.1**

- Updated README

**v2.4.0**

- Updated mod for the Scorched Acres update!
- Added options to toggle inheritance for each individual type of drone (all default to true)
- Both Incinerator & Equipment drones can now inherit player items
- UpdateInventory should now update drones to the player who bought them

**v2.3.1** 

- Fixed a generator setting bug (was using GenCap for GenChance)

**v2.3.0** 

- Updated mod to most recent version of BepInEx and RoR2 patch!
- Added option to toggle either drones or turrets inheritance (both default is set to true)

**v2.2.0** 

- Added option for drones to have their inventories updated to what the host has (default is set to false)

**v2.1.0** 

- Added item generation and its respective options (default is set to false)
- Added option to inherit equipment. Only works with Queen Guards and Ghosts (default is set to false)
- Added float/decimal handling for ItemMultiplier. Possible to now have drones inherit half your items! (default is set to 1)
- Added ItemRandomizer option (default is set to false)
- Added option to have drones and turrets inherit items (default is set to true)
- Added option for Back-up drones to inherit items (default is set to true)
- Added proper fix and option for Back-up drones to reinherit 25s death timer upon Dio's revive (default is set to true)

**v2.0.0**

- Added configuration
- Queen Guards can now inherit items (default is set to false)
- Ghosts from Happiest Mask can now inherit items (default is set to false)

**v1.1.1**

- Updated README

**v1.1.0**

- "The Back-up" drones now get player items

**v1.0.0**

- All purchaseable drones and turrets now get player items at time of purchase!

## Other mods

[EnemiesWithItems](https://thunderstore.io/package/BasilPanda/EnemiesWithItems/)
[RemoveAllyCap](https://thunderstore.io/package/BasilPanda/RemoveAllyCap/)