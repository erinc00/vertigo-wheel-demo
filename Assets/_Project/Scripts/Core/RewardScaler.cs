using UnityEngine;

namespace Vertigo.Core
{
    public sealed class RewardScaler
    {
        private readonly ZoneProgressionConfig _config;

        public RewardScaler(ZoneProgressionConfig config) => _config = config;

        public RewardInstance Create(RewardData data, int zone)
        {
            float mult = _config != null ? _config.GetRewardMultiplier(zone) : 1f;
            long amount = (long)Mathf.Round(data.BaseAmount * mult);
            return new RewardInstance(data, amount);
        }
    }
}
