# How to QuickSell

<p align="center">
  <a href="https://github.com/KeyErrorFinn/how-to-fish-quicksell-mod/commits/main"><img alt="GitHub last commit" src="https://img.shields.io/github/last-commit/KeyErrorFinn/how-to-fish-quicksell-mod" /></a>
  <img alt="C Sharp" src="https://img.shields.io/badge/C%23-512BD4?logo=dotnet&logoColor=fff" />
  <img alt=".NET Framework 4.7.2" src="https://img.shields.io/badge/.NET%20Framework-4.7.2-512BD4" />
  <img alt="BepInEx 5" src="https://img.shields.io/badge/BepInEx-5-222" />
  <img alt="Thunderstore" src="https://img.shields.io/badge/Thunderstore-package-1F87FF" />
</p>

A BepInEx mod for *How to Fish* that sells eligible catches through the game's normal trader flow. Press `F8` to sell the enabled groups.

## Features

- Sells eligible fish and creatures from the inventory.
- Includes a freshly held catch that has not entered the inventory bar.
- Can optionally sell nearby dead floor fish.
- Uses the game's own money and item-destruction flow.
- Protects quest items, zero-value items, and living creatures.
- Adds inventory and floor-selling toggles to **Options > Gameplay**.
- Adds scrolling to the Gameplay settings list when needed.

## Requirements

- *How to Fish*
- BepInEx 5
- Single-player or lobby-host authority
- A loaded island containing a money NPC

Remote multiplayer clients cannot perform the sale.

## Install

1. Download `KeyErrorFinn.QuickSell.dll` from the release.
2. Create:

   ~~~text
   BepInEx/plugins/KeyErrorFinn-QuickSell/
   ~~~

3. Put the DLL in that folder.
4. Start the game through the same BepInEx or Thunderstore profile.
5. Check the BepInEx log for:

   ~~~text
   How to QuickSell 1.0.0 loaded
   ~~~

## Controls and settings

The default hotkey is `F8`.

| Setting | Default | Description |
| --- | --- | --- |
| QuickSell: Floor Fish | Off | Sell eligible dead fish on the island and in nearby water. |
| QuickSell: Inventory Fish/Creatures | On | Sell eligible inventory catches and the freshly held item. |
| Sell all hotkey | F8 | Run one sale pass for every enabled group. |
| Only sell fish | On | Restrict inventory sales to fish and creature items. |
| Floor sale water buffer | 30 metres | Extend the floor-fish search beyond the island edge. |

The two everyday toggles appear in **Options > Gameplay**. Advanced values are stored in BepInEx's configuration for `com.keyerrorfinn.quicksell`.

> [!CAUTION]
> Turning off **Only sell fish** allows any non-quest inventory item with a positive value to be sold. Test with low-value items first.

## Eligibility rules

An inventory item is skipped when it:

- is a quest item,
- has no sale value,
- contains a living creature,
- is already being destroyed or sold,
- or does not match the fish-only filter.

Floor selling is stricter. It only selects dead `Fish` objects that are outside an inventory, have no holder, are within the configured island range, and have a positive value. Shellfish and other non-fish floor creatures are excluded.

## Build from source

1. Copy `GameReferences.props.example` to `GameReferences.props`.
2. Set `HowToFishGameDir` and `BepInExDir` for your installation.
3. Build the project:

   ~~~powershell
   dotnet build QuickSell.csproj -c Release
   ~~~

The project targets .NET Framework 4.7.2. `GameReferences.props` is ignored by Git so machine-specific paths stay local.

For ScriptEngine development:

~~~powershell
./build-and-reload.ps1
~~~

That script builds with `DeployToProfile=true` and copies the matching DLL and PDB into `BepInEx/scripts` because `DeployToScriptEngine` defaults to true.

## Troubleshooting

| Message or symptom | Check |
| --- | --- |
| No money NPC is available | Travel to an island with the trader and try again. |
| Nothing sells in multiplayer | Run as the lobby host or in single-player. |
| Floor fish are ignored | Enable floor selling and confirm the fish is dead and near the loaded island. |
| A catch is protected | Check whether it is a quest item, alive, worthless, or blocked by the fish-only filter. |
| Settings do not appear | Confirm the DLL is loaded by the same BepInEx profile used to start the game. |

## Project layout

- `src/QuickSellPlugin.cs`, configuration, hotkey, and settings registration.
- `src/QuickSellService.cs`, sale orchestration.
- `src/SaleRules.cs`, eligibility and money-NPC lookup.
- `src/QuickSellSettingsUI.cs`, native Gameplay menu controls.
- `dist/`, Thunderstore package contents.
- `build-and-reload.ps1`, ScriptEngine build and deployment helper.

## Version

Current source and package version: **1.0.0**

See `dist/CHANGELOG.md` for release notes.

## Author

Created by [KeyErrorFinn](https://github.com/KeyErrorFinn).

## Licence

No project-level licence is currently included. BepInEx, the game, and other dependencies keep their own licences.
