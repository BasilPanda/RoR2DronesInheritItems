# Drones Inherit Items
## By Basil
A mod that allows **all** drones and turrets (and then some) to inherit items from the player when purchased.

**Has QUEEN GUARDS, GHOSTS, and lots of configuration!** 

**Don't want to inherit player items but still want things to have items? Now possible with the ItemGenerator option in the config file!**
*Will ignore inherit settings.*

**Now has an update drone inventory option!**

All drones and turrets are affected by this mod. It will only inherit the items from a player's inventory at the time of purchase or activation.

## Installation
1. Install [BepInEx Mod Pack](https://thunderstore.io/package/bbepis/BepInExPack/)
2. Visit the [releases page](https://github.com/BasilPanda/RoR2DronesInheritItems/releases/) or get it [here](https://thunderstore.io/package/BasilPanda/DronesInheritItems/).
3. Download the latest DronesInheritItems.dll
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

Q: How do you calculate the item max cap for item generation?

A: Currently the way it is done is randomly selecting a value from 0 to the current stage * item max cap, inclusively, for every item.

Q: How does the ItemRandomizer setting work?

A: It will randomly choose a value between 0 and the player's item count for the respective item if the setting is true.

## Future Plans

- Allow configuration of settings with an ingame menu! If someone knows how to help, please contact me on Discord!

## Changelog

**v2.2.0** UPDATING DRONES PATCH

- Added option for drones to have their inventories updated to what the host has (default is set to false)

**v2.1.0** EXTRA CONFIG PATCH!

- Added item generation and its respective options (default is set to false)

- Added option to inherit equipment. Only works with Queen Guards and Ghosts. (default is set to false)

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
