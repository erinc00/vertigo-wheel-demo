namespace Vertigo.Core
{
    public sealed class ZoneRules
    {
        private readonly int _safeInterval;
        private readonly int _superInterval;

        public ZoneRules(ZoneRuleSettings settings)
        {
            _safeInterval = settings?.SafeZoneInterval ?? 5;
            _superInterval = settings?.SuperZoneInterval ?? 30;
        }

        public ZoneType GetZoneType(int zone)
        {
            if (zone % _superInterval == 0) return ZoneType.Super;
            if (zone % _safeInterval == 0) return ZoneType.Safe;
            return ZoneType.Normal;
        }

        public bool HasBomb(int zone) => GetZoneType(zone) == ZoneType.Normal;
    }
}
