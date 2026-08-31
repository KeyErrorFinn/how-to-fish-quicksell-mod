using System.Collections.Generic;
using System.Linq;
using BepInEx.Logging;
using UnityEngine;

namespace KeyErrorFinn.QuickSell
{
    internal static class SaleRules
    {
        public static bool IsEligibleForSale(Item item, bool onlySellFish)
        {
            if (item == null || item.IsQuestItem || item.TotalWorth <= 0 || (item.Creature != null && !item.Creature.IsDead))
                return false;

            return !onlySellFish || item.Fish != null || item.Creature != null;
        }

        public static IEnumerable<Item> GetFloorFishNearIsland(float waterBuffer, ManualLogSource log)
        {
            if (Island.CurIsland == null)
            {
                log.LogWarning("No island is loaded, so floor-item selling was skipped.");
                return Enumerable.Empty<Item>();
            }

            var range = Mathf.Max(0f, Island.IslandSize + waterBuffer);
            var rangeSquared = range * range;
            return ItemManager.Items.Values
                .Where(item => item != null
                    && item.Fish != null
                    && item.Fish.IsDead
                    && !item.IsQuestItem
                    && !item.IsInInventory
                    && item.Holder == null
                    && item.SyncedHolder == null
                    && !item.IsDestroying
                    && !item.IsDeinitializing
                    && item.TotalWorth > 0)
                .Where(item => (item.transform.position - Island.IslandPos).sqrMagnitude <= rangeSquared)
                .Distinct()
                .ToList();
        }

        public static NPC FindMoneyNpc()
        {
            return Object.FindObjectsByType<NPC>()
                .FirstOrDefault(npc => npc != null && npc.Quests.Any(quest => quest != null && quest.Type == QuestType.Money));
        }
    }
}
