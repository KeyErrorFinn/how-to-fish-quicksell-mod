# How to QuickSell

How to QuickSell is a BepInEx mod for *How to Fish* that sells your catch quickly through the game's normal sale path. Press one key to sell the enabled groups, with the money and item removal handled by the game itself.

## Features

- Sell eligible fish and creatures from your inventory with `F8`.
- Sell a freshly caught item that is still being held and has not entered the inventory bar yet.
- Optionally sell dead fish lying on the current island or floating in nearby water.
- Uses the game's trader sale flow, so items are removed and money is awarded normally.
- Protects quest items, zero-value items, and living creatures.
- Adds two toggles to **Options > Gameplay**:
  - **QuickSell: Floor Fish**
  - **QuickSell: Inventory Fish/Creatures**
- The Gameplay settings list scrolls and has a scrollbar when the options do not fit on screen.

## How it works

QuickSell finds the trader's money NPC on the loaded island and sends each selected item through the game's own sale and destruction logic. This means it must be used by the lobby host / single-player host; it cannot sell items on a remote server from a multiplayer client.

By default, inventory fish/creatures are enabled and floor fish are disabled. Press `F8` to sell every enabled group. For floor selling, only dead `Fish` objects close to the loaded island are included - clams, leeches, footsnails, and other non-fish pickups are excluded.

## Installation

### Normal installation

1. Install **BepInEx 5** for *How to Fish* (the Thunderstore profile is the simplest option).
2. Download `KeyErrorFinn.QuickSell.dll` from the QuickSell release.
3. Create this folder if it does not already exist:

   `BepInEx/plugins/KeyErrorFinn-QuickSell/`

4. Copy `KeyErrorFinn.QuickSell.dll` into that folder.
5. Start the game through the same Thunderstore profile.

## Settings

The two everyday settings are in **Options > Gameplay**:

| Setting | Default | Description |
| --- | --- | --- |
| QuickSell: Floor Fish | Off | Sell dead fish on the loaded island and within the nearby-water range. |
| QuickSell: Inventory Fish/Creatures | On | Sell eligible inventory catches and an eligible catch currently held in hand. |

Additional advanced settings are stored in BepInEx's config file for `com.keyerrorfinn.quicksell`:

- **Sell all hotkey**  -  defaults to `F8`.
- **Only sell fish**  -  keeps the safety filter enabled by default.
- **Floor sale water buffer**  -  defaults to 30 metres beyond the island edge.

## Requirements and notes

- *How to Fish* with BepInEx 5.
- You must be in single-player or be the lobby host.
- A money NPC must be loaded; travel to the trader island if QuickSell says none is available.
- Test mod updates with low-value catches first, especially after a game update.

## Author

Created by [KeyErrorFinn](https://github.com/KeyErrorFinn).

## Version

**1.0.0**
