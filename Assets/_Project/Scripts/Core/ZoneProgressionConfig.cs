using System.Collections.Generic;
using UnityEngine;

namespace Vertigo.Core
{
    [CreateAssetMenu(menuName = "Vertigo/Zone Progression", fileName = "zone_progression")]
    public sealed class ZoneProgressionConfig : ScriptableObject
    {
        [SerializeField] private ZoneRuleSettings ruleSettings = new ZoneRuleSettings();

        [SerializeField] private List<ZoneTierEntry> normalTiers = new();
        [SerializeField] private List<ZoneTierEntry> safeTiers = new();
        [SerializeField] private List<ZoneTierEntry> superTiers = new();

        [SerializeField, Min(0f)] private float growthPerZone = 0.15f;
        [SerializeField] private bool useCurve = false;
        [SerializeField] private AnimationCurve multiplierCurve = AnimationCurve.Linear(0f, 1f, 1f, 10f);
        [SerializeField, Min(1)] private int curveMaxZone = 60;

        public ZoneRuleSettings RuleSettings => ruleSettings;

        public WheelConfig GetWheel(ZoneType type, int zone)
        {
            List<ZoneTierEntry> tiers = type switch
            {
                ZoneType.Safe => safeTiers,
                ZoneType.Super => superTiers,
                _ => normalTiers
            };
            return ResolveTier(tiers, zone);
        }

        private static WheelConfig ResolveTier(List<ZoneTierEntry> tiers, int zone)
        {
            WheelConfig best = null;
            int bestMin = int.MinValue;
            for (int i = 0; i < tiers.Count; i++)
            {
                var t = tiers[i];
                if (t == null || t.Wheel == null) continue;
                if (t.MinZone <= zone && t.MinZone > bestMin)
                {
                    bestMin = t.MinZone;
                    best = t.Wheel;
                }
            }
            if (best == null)
            {
                for (int i = 0; i < tiers.Count; i++)
                {
                    if (tiers[i]?.Wheel != null)
                    {
                        best = tiers[i].Wheel;
                        break;
                    }
                }
            }
            return best;
        }

        public float GetRewardMultiplier(int zone)
        {
            if (zone < 1) zone = 1;
            if (useCurve)
                return Mathf.Max(0f, multiplierCurve.Evaluate((float)zone / curveMaxZone));
            return 1f + (zone - 1) * growthPerZone;
        }
    }
}