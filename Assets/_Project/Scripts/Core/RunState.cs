using System;
using System.Collections.Generic;

namespace Vertigo.Core
{
    public sealed class RunState
    {
        private readonly List<RewardInstance> _collected = new List<RewardInstance>();
        private readonly Dictionary<string, RewardInstance> _byId = new Dictionary<string, RewardInstance>();

        public int CurrentZone { get; private set; } = 1;
        public IReadOnlyList<RewardInstance> Collected => _collected;

        public event Action Changed;

        public void Reset()
        {
            CurrentZone = 1;
            _collected.Clear();
            _byId.Clear();
            Changed?.Invoke();
        }

        public void AdvanceZone()
        {
            CurrentZone++;
            Changed?.Invoke();
        }

        public void AddReward(RewardInstance reward)
        {
            if (reward == null) return;

            if (reward.Stackable && _byId.TryGetValue(reward.Id, out var existing))
            {
                existing.Add(reward.Amount);
            }
            else
            {
                _collected.Add(reward);
                if (reward.Stackable) _byId[reward.Id] = reward;
            }
            Changed?.Invoke();
        }

        public void ClearCollected()
        {
            _collected.Clear();
            _byId.Clear();
            Changed?.Invoke();
        }
    }
}
