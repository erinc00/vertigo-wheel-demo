using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Vertigo.Core;

namespace Vertigo.Tests
{
    public sealed class WheelResultSelectorTests
    {
        [SetUp]
        public void SetUp() => LogAssert.ignoreFailingMessages = true;

        private sealed class FixedRandom : IRandomProvider
        {
            private readonly int _value;
            public FixedRandom(int value) => _value = value;
            public int Next(int maxExclusive) => _value % maxExclusive;
            public float NextFloat(float min, float max) => min;
        }

        private static WheelConfig MakeConfig(params int[] weights)
        {
            var cfg = ScriptableObject.CreateInstance<WheelConfig>();
            var list = new System.Collections.Generic.List<WheelSliceData>();
            foreach (var w in weights)
            {
                var slice = new WheelSliceData();
                typeof(WheelSliceData).GetField("weight",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    .SetValue(slice, w);
                list.Add(slice);
            }
            typeof(WheelConfig).GetField("slices",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(cfg, list);
            return cfg;
        }

        [Test]
        public void Select_RespectsWeights_FirstBucket()
        {
            var cfg = MakeConfig(1, 1, 1, 1);
            var selector = new WeightedWheelResultSelector(new FixedRandom(0));
            Assert.AreEqual(0, selector.Select(cfg).SliceIndex);
        }

        [Test]
        public void Select_RespectsWeights_LastBucket()
        {
            var cfg = MakeConfig(1, 1, 1, 1);
            var selector = new WeightedWheelResultSelector(new FixedRandom(3));
            Assert.AreEqual(3, selector.Select(cfg).SliceIndex);
        }

        [Test]
        public void Select_HeavyWeight_DominatesRoll()
        {
            var cfg = MakeConfig(1, 100);
            var selector = new WeightedWheelResultSelector(new FixedRandom(50));
            Assert.AreEqual(1, selector.Select(cfg).SliceIndex);
        }
    }
}
