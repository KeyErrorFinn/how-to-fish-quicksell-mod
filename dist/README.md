# How to QuickSell

How to QuickSell is a BepInEx mod for *How to Fish* that sells your catch quickly through the game's normal trader flow. Press one key to sell the enabled item groups, with money and item removal handled by the game itself.

## Features

- Sell eligible fish and creatures from your inventory with **F8**.
- Sell a freshly caught item that is still held outside the inventory bar.
- Optionally sell dead fish on the current island or floating in nearby water.
- Uses the game's trader sale flow for normal money and item removal.
- Protects quest items, zero-value items, living creatures, and items already being sold.
- Adds native toggles to **Options > Gameplay**.
- Makes the Gameplay options list scroll when the added settings do not fit.

## Installation

### Mod manager

Install How to QuickSell with Thunderstore Mod Manager or r2modman. BepInEx will be installed automatically as a dependency. Start *How to Fish* using the modded launch button.

### Manual

1. Install BepInEx 5 x64 for *How to Fish* and run the game once.
2. Create `BepInEx/plugins/KeyErrorFinn-QuickSell/`.
3. Copy `KeyErrorFinn.QuickSell.dll` from this package into that folder.
4. Start the game.

## Controls and settings

Press **F8** to sell every enabled group.

The everyday settings appear in **Options > Gameplay**:

| Setting | Default | Description |
| --- | --- | --- |
| QuickSell: Floor Fish | Off | Sell dead fish on the island and within the nearby-water range. |
| QuickSell: Inventory Fish/Creatures | On | Sell eligible inventory catches and an eligible catch held in hand. |

Additional settings are saved in `BepInEx/config/com.keyerrorfinn.quicksell.cfg`:

- **Sell all hotkey** — defaults to F8.
- **Only sell fish** — prevents ordinary non-creature items from being sold by default.
- **Floor sale water buffer** — defaults to 30 metres beyond the island edge.

## Compatibility and limitations

- Requires BepInEx 5 for *How to Fish*.
- Must be used in single-player or by the lobby host; remote clients cannot sell through the host's server state.
- A money NPC must be loaded. Travel to the trader island if the mod reports that none is available.
- Test updates with low-value catches first after a game update.
