using NUnit.Framework;
using UnityEngine;
using Vertigo.Core;

namespace Vertigo.Tests
{
    public sealed class RunStateTests
    {
        private static RewardData MakeReward(string id, RewardKind kind, long amount, bool stackable)
        {
            var r = ScriptableObject.CreateInstance<RewardData>();
            var t = typeof(RewardData);
            void Set(string f, object v) => t.GetField(f,
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(r, v);
            Set("id", id); Set("kind", kind); Set("baseAmount", amount); Set("stackable", stackable);
            return r;
        }

        [Test]
        public void AddReward_StacksSameId()
        {
            var run = new RunState();
            var gold = MakeReward("gold", RewardKind.Gold, 100, true);
            run.AddReward(new RewardInstance(gold, 100));
            run.AddReward(new RewardInstance(gold, 50));
            Assert.AreEqual(1, run.Collected.Count);
            Assert.AreEqual(150, run.Collected[0].Amount);
        }

        [Test]
        public void AddReward_NonStackable_DoesNotMerge()
        {
            var run = new RunState();
            var skin = MakeReward("skin", RewardKind.Skin, 1, false);
            run.AddReward(new RewardInstance(skin, 1));
            run.AddReward(new RewardInstance(skin, 1));
            Assert.AreEqual(2, run.Collected.Count);
        }

        [Test]
        public void ClearCollected_EmptiesTempRewards()
        {
            var run = new RunState();
            run.AddReward(new RewardInstance(MakeReward("g", RewardKind.Gold, 10, true), 10));
            run.ClearCollected();
            Assert.AreEqual(0, run.Collected.Count);
        }

        [Test]
        public void Reset_ReturnsToZoneOne()
        {
            var run = new RunState();
            run.AdvanceZone();
            run.AdvanceZone();
            Assert.AreEqual(3, run.CurrentZone);
            run.Reset();
            Assert.AreEqual(1, run.CurrentZone);
        }

        [Test]
        public void Wallet_Commit_AddsCurrency_AndKeepsItems()
        {
            var wallet = new Wallet();
            var run = new RunState();
            run.AddReward(new RewardInstance(MakeReward("gold", RewardKind.Gold, 100, true), 100));
            run.AddReward(new RewardInstance(MakeReward("skin", RewardKind.Skin, 1, false), 1));
            wallet.Commit(run.Collected);
            Assert.AreEqual(100, wallet.Gold);
            Assert.AreEqual(1, wallet.PermanentItems.Count);
        }

        [Test]
        public void Wallet_SpendGold_FailsWhenInsufficient()
        {
            var wallet = new Wallet(20);
            Assert.IsFalse(wallet.SpendGold(25));
            Assert.AreEqual(20, wallet.Gold);
            Assert.IsTrue(wallet.SpendGold(15));
            Assert.AreEqual(5, wallet.Gold);
        }
    }
}
