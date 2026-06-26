using System;
using UnityEngine;

namespace Vertigo.Core
{
    [Serializable]
    public sealed class ZoneRuleSettings
    {
        [field: SerializeField, Min(1)] public int SafeZoneInterval { get; private set; } = 5;
        [field: SerializeField, Min(1)] public int SuperZoneInterval { get; private set; } = 30;

        public ZoneRuleSettings() { }

        public ZoneRuleSettings(int safeInterval, int superInterval)
        {
            SafeZoneInterval = Mathf.Max(1, safeInterval);
            SuperZoneInterval = Mathf.Max(1, superInterval);
        }
    }
}
