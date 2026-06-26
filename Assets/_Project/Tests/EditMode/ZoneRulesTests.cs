using NUnit.Framework;
using Vertigo.Core;

namespace Vertigo.Tests
{
    public sealed class ZoneRulesTests
    {
        private ZoneRules _rules;

        [SetUp]
        public void SetUp() => _rules = new ZoneRules(new ZoneRuleSettings(5, 30));

        [TestCase(1, ZoneType.Normal)]
        [TestCase(4, ZoneType.Normal)]
        [TestCase(5, ZoneType.Safe)]
        [TestCase(10, ZoneType.Safe)]
        [TestCase(25, ZoneType.Safe)]
        [TestCase(29, ZoneType.Normal)]
        [TestCase(30, ZoneType.Super)]
        [TestCase(60, ZoneType.Super)]
        [TestCase(90, ZoneType.Super)]
        public void GetZoneType_ReturnsExpected(int zone, ZoneType expected)
            => Assert.AreEqual(expected, _rules.GetZoneType(zone));

        [Test]
        public void NormalZones_HaveBomb_SafeAndSuper_DoNot()
        {
            Assert.IsTrue(_rules.HasBomb(1));
            Assert.IsFalse(_rules.HasBomb(5));
            Assert.IsFalse(_rules.HasBomb(30));
        }
    }
}
