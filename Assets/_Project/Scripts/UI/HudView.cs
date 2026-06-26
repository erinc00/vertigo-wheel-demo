using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Vertigo.Core;

namespace Vertigo.UI
{
    public sealed class HudView : MonoBehaviour
    {
        [SerializeField] private RectTransform ui_group_at_risk_items;
        [SerializeField] private CollectedItemRowView atRiskRowPrefab;

        [SerializeField] private RectTransform ui_group_inventory_items;
        [SerializeField] private CollectedItemRowView inventoryRowPrefab;
        [SerializeField] private Sprite goldIcon;
        [SerializeField] private Sprite cashIcon;

        private readonly List<CollectedItemRowView> _atRiskRows = new();
        private readonly Dictionary<string, (string name, Sprite icon, long amount)> _atRiskGrouped = new();

        private readonly List<CollectedItemRowView> _invRows = new();
        private readonly Dictionary<string, (string name, Sprite icon, long amount)> _invGrouped = new();

        public void SetAtRisk(IReadOnlyList<RewardInstance> rewards)
        {
            _atRiskGrouped.Clear();
            for (int i = 0; i < rewards.Count; i++)
            {
                var r = rewards[i];
                if (_atRiskGrouped.TryGetValue(r.Id, out var existing))
                    _atRiskGrouped[r.Id] = (existing.name, existing.icon, existing.amount + r.Amount);
                else
                    _atRiskGrouped[r.Id] = (r.DisplayName, r.Icon, r.Amount);
            }
            RenderRows(_atRiskGrouped, ui_group_at_risk_items, atRiskRowPrefab, _atRiskRows);
        }

        public void SetInventory(long gold, long cash, IReadOnlyList<RewardInstance> permanentItems)
        {
            _invGrouped.Clear();
            _invGrouped["__gold"] = ("Gold", goldIcon, gold);
            _invGrouped["__cash"] = ("Cash", cashIcon, cash);
            for (int i = 0; i < permanentItems.Count; i++)
            {
                var r = permanentItems[i];
                if (_invGrouped.TryGetValue(r.Id, out var existing))
                    _invGrouped[r.Id] = (existing.name, existing.icon, existing.amount + r.Amount);
                else
                    _invGrouped[r.Id] = (r.DisplayName, r.Icon, r.Amount);
            }
            RenderRows(_invGrouped, ui_group_inventory_items, inventoryRowPrefab, _invRows);
        }

        private void RenderRows(
            Dictionary<string, (string name, Sprite icon, long amount)> grouped,
            RectTransform container,
            CollectedItemRowView prefab,
            List<CollectedItemRowView> rows)
        {
            if (container == null || prefab == null) return;

            while (rows.Count < grouped.Count)
                rows.Add(Instantiate(prefab, container));

            int idx = 0;
            foreach (var entry in grouped)
            {
                rows[idx].gameObject.SetActive(true);
                rows[idx].Set(entry.Value.icon, $"{entry.Value.name}: {entry.Value.amount}");
                idx++;
            }
            for (; idx < rows.Count; idx++) rows[idx].gameObject.SetActive(false);
        }
    }
}