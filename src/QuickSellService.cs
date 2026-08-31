using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace KeyErrorFinn.QuickSell
{
    internal sealed class QuickSellService
    {
        private readonly ManualLogSource _log;
        private readonly ConfigEntry<bool> _sellFloorItems;
        private readonly ConfigEntry<bool> _sellInventoryItems;
        private readonly ConfigEntry<bool> _onlySellFish;
        private readonly ConfigEntry<float> _floorSaleWaterBuffer;
        private readonly FieldInfoCache _fields = new FieldInfoCache();
        private readonly HashSet<Item> _itemsBeingSold = new HashSet<Item>();

        public QuickSellService(ManualLogSource log, ConfigEntry<bool> sellFloorItems, ConfigEntry<bool> sellInventoryItems,
            ConfigEntry<bool> onlySellFish, ConfigEntry<float> floorSaleWaterBuffer)
        {
            _log = log;
            _sellFloorItems = sellFloorItems;
            _sellInventoryItems = sellInventoryItems;
            _onlySellFish = onlySellFish;
            _floorSaleWaterBuffer = floorSaleWaterBuffer;
        }

        public void TrySell()
        {
            // DestroyItem is asynchronous, so a just-sold item can remain visible to
            // the inventory/world queries for several frames. Keep it locked until
            // Unity has actually destroyed it, preventing repeated money awards.
            _itemsBeingSold.RemoveWhere(item => item == null);

            if (Player.LocalPlayer == null || Player.LocalPlayer.Inventory == null)
            {
                _log.LogWarning("QuickSell is only available after the local player has spawned.");
                return;
            }

            if (Server.Instance == null || !Server.Instance.IsServerInitialized)
            {
                _log.LogWarning("QuickSell must be used by the lobby host. It cannot change a remote server's inventory or money.");
                return;
            }

            var inventory = Player.LocalPlayer.Inventory;
            var moneyNpc = SaleRules.FindMoneyNpc();
            if (moneyNpc == null)
            {
                _log.LogWarning("No money NPC is loaded. Move to the trader island before using QuickSell.");
                return;
            }

            var slots = _fields.GetSlots(inventory);
            if (slots == null)
            {
                _log.LogError("Unable to read the game's inventory slots; the game may have changed.");
                return;
            }

            var items = CollectItems(inventory, slots, out var heldItem, out var heldItemIsOutsideInventory);
            if (items.Count == 0)
            {
                _log.LogInfo("No eligible inventory items to sell.");
                return;
            }

            foreach (var item in items)
                _itemsBeingSold.Add(item);

            try
            {
                if (_sellInventoryItems.Value)
                    inventory.ApplySlot(-1);

                if (heldItemIsOutsideInventory)
                    Player.LocalPlayer.Holding.DropItem(calledFromLocal: false, droppedItem: heldItem);

                foreach (var item in items)
                {
                    if (inventory.HasItemInInventory(item))
                        inventory.RemoveItem(item);

                    MoneyManager.SellItem(item);
                    item.DestroyItem(2, moneyNpc.ID);
                }

                _log.LogInfo($"Sold {items.Count} item(s) through money NPC {moneyNpc.ID}.");
            }
            catch (Exception exception)
            {
                _log.LogError($"QuickSell stopped: {exception}");
            }
        }

        private List<Item> CollectItems(PlayerInventory inventory, List<InventorySlot> slots, out Item heldItem,
            out bool heldItemIsOutsideInventory)
        {
            var items = new List<Item>();
            if (_sellInventoryItems.Value)
            {
                items.AddRange(slots
                    .Select(slot => slot == null ? null : slot.Item)
                    .Where(item => SaleRules.IsEligibleForSale(item, _onlySellFish.Value) && !_itemsBeingSold.Contains(item)));
            }

            heldItem = Player.LocalPlayer.Holding.HeldItem;
            heldItemIsOutsideInventory = _sellInventoryItems.Value
                && SaleRules.IsEligibleForSale(heldItem, _onlySellFish.Value)
                && !_itemsBeingSold.Contains(heldItem)
                && !inventory.HasItemInInventory(heldItem);
            if (heldItemIsOutsideInventory)
                items.Add(heldItem);

            if (_sellFloorItems.Value)
                items.AddRange(SaleRules.GetFloorFishNearIsland(_floorSaleWaterBuffer.Value, _log)
                    .Where(item => !_itemsBeingSold.Contains(item)));

            return items.Distinct().ToList();
        }

        private sealed class FieldInfoCache
        {
            private readonly System.Reflection.FieldInfo _itemSlots = AccessTools.Field(typeof(PlayerInventory), "_itemSlots");

            public List<InventorySlot> GetSlots(PlayerInventory inventory)
            {
                return _itemSlots?.GetValue(inventory) as List<InventorySlot>;
            }
        }
    }
}
