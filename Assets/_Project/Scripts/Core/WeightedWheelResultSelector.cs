using UnityEngine;

namespace Vertigo.Core
{
    public sealed class WeightedWheelResultSelector : IWheelResultSelector
    {
        private readonly IRandomProvider _random;

        public WeightedWheelResultSelector(IRandomProvider random) => _random = random;

        public WheelResult Select(WheelConfig config)
        {
            var slices = config.Slices;
            int count = slices.Count;

            int totalWeight = 0;
            for (int i = 0; i < count; i++)
                totalWeight += Mathf.Max(0, slices[i].Weight);

            if (totalWeight <= 0)
            {
                int idx = _random.Next(count);
                var s = slices[idx];
                return new WheelResult(idx, s.IsBomb, s.Reward);
            }

            int roll = _random.Next(totalWeight);
            int acc = 0;
            for (int i = 0; i < count; i++)
            {
                acc += Mathf.Max(0, slices[i].Weight);
                if (roll < acc)
                    return new WheelResult(i, slices[i].IsBomb, slices[i].Reward);
            }

            int last = count - 1;
            return new WheelResult(last, slices[last].IsBomb, slices[last].Reward);
        }
    }
}
