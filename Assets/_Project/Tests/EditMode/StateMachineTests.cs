using NUnit.Framework;
using Vertigo.Core;

namespace Vertigo.Tests
{
    public sealed class StateMachineTests
    {
        [Test]
        public void CanSpin_OnlyInWaitingForSpin()
        {
            var sm = new GameStateMachine();
            sm.ChangeState(GameState.WaitingForSpin);
            Assert.IsTrue(sm.CanSpin);
            sm.ChangeState(GameState.Spinning);
            Assert.IsFalse(sm.CanSpin);
        }

        [Test]
        public void CanLeave_OnlyInSafeOrSuper_AndNotSpinning()
        {
            var sm = new GameStateMachine();
            sm.ChangeState(GameState.WaitingForSpin);
            Assert.IsFalse(sm.CanLeave(ZoneType.Normal));
            Assert.IsTrue(sm.CanLeave(ZoneType.Safe));
            Assert.IsTrue(sm.CanLeave(ZoneType.Super));

            sm.ChangeState(GameState.Spinning);
            Assert.IsFalse(sm.CanLeave(ZoneType.Safe));
        }
    }
}
