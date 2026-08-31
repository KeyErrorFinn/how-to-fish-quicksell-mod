using BepInEx;
using BepInEx.Configuration;
using UnityEngine;

namespace KeyErrorFinn.QuickSell
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public sealed class QuickSellPlugin : BaseUnityPlugin
    {
        public const string PluginGuid = "com.keyerrorfinn.quicksell";
        public const string PluginName = "How to QuickSell";
        public const string PluginVersion = "1.0.0";

        private ConfigEntry<KeyboardShortcut> _sellAllHotkey;
        private ConfigEntry<bool> _sellFloorItems;
        private ConfigEntry<bool> _sellInventoryItems;
        private ConfigEntry<bool> _onlySellFish;
        private ConfigEntry<float> _floorSaleWaterBuffer;
        private QuickSellService _quickSell;
        private QuickSellSettingsUI _settingsUi;

        private void Awake()
        {
            _sellAllHotkey = Config.Bind("General", "Sell all hotkey", new KeyboardShortcut(KeyCode.F8),
                "Sell all enabled item groups. You must be the host/server.");
            _sellFloorItems = Config.Bind("Sale options", "Sell all floor items", false,
                "Sell dead fish on the loaded island and in the nearby water. Shellfish and other non-fish creatures are excluded.");
            _sellInventoryItems = Config.Bind("Sale options", "Sell all inventory items", true,
                "Sell eligible items in the local inventory, including a freshly picked-up item held outside the inventory bar.");
            _onlySellFish = Config.Bind("Safety", "Only sell fish", true,
                "When enabled, only fish/creature items are sold. Turn this off to sell all non-quest items with value.");
            _floorSaleWaterBuffer = Config.Bind("Sale options", "Floor sale water buffer", 30f,
                "Additional distance beyond the island edge for dead fish floating in nearby water.");

            _quickSell = new QuickSellService(
                Logger,
                _sellFloorItems,
                _sellInventoryItems,
                _onlySellFish,
                _floorSaleWaterBuffer);
            _settingsUi = new QuickSellSettingsUI(Logger);

            Logger.LogInfo($"{PluginName} {PluginVersion} loaded. Press {_sellAllHotkey.Value} to sell enabled items.");
            _settingsUi.Install(_sellFloorItems, _sellInventoryItems);
        }

        private void Update()
        {
            if (!_sellAllHotkey.Value.IsDown())
                return;

            _quickSell.TrySell();
        }

        private void OnDestroy()
        {
            _settingsUi?.Dispose();
        }
    }
}
