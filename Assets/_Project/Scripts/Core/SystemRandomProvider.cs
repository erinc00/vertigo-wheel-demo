using System;

namespace Vertigo.Core
{
    public sealed class SystemRandomProvider : IRandomProvider
    {
        private readonly Random _random;

        public SystemRandomProvider() => _random = new Random();
        public SystemRandomProvider(int seed) => _random = new Random(seed);

        public int Next(int maxExclusive) => _random.Next(maxExclusive);
        public float NextFloat(float min, float max) => min + (float)_random.NextDouble() * (max - min);
    }
}
