using System;

namespace Vertigo.Core
{
    public sealed class GameStateMachine
    {
        public GameState Current { get; private set; } = GameState.PreparingZone;

        public event Action<GameState> StateChanged;

        public void ChangeState(GameState next)
        {
            if (Current == next) return;
            Current = next;
            StateChanged?.Invoke(next);
        }

        public bool CanSpin => Current == GameState.WaitingForSpin;

        public bool CanLeave(ZoneType zoneType)
            => Current == GameState.WaitingForSpin &&
               (zoneType == ZoneType.Safe || zoneType == ZoneType.Super);
    }
}
