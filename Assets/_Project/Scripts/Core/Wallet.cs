using System;
using System.Collections.Generic;

namespace Vertigo.Core
{
    public sealed class Wallet
    {
        private readonly List<RewardInstance> _permanentItems = new List<RewardInstance>();

        public long Gold { get; private set; }
        public long Cash { get; private set; }
        public IReadOnlyList<RewardInstance> PermanentItems => _permanentItems;

        public event Action Changed;

        public Wallet(long startGold = 0, long startCash = 0)
        {
            Gold = startGold;
            Cash = startCash;
        }

        public bool CanAfford(long gold) => Gold >= gold;

        public bool SpendGold(long amount)
        {
            if (amount < 0 || Gold < amount) return false;
            Gold -= amount;
            Changed?.Invoke();
            return true;
        }

        public void Commit(IReadOnlyList<RewardInstance> rewards)
        {
            if (rewards == null) return;
            for (int i = 0; i < rewards.Count; i++)
            {
                var r = rewards[i];
                switch (r.Kind)
                {
                    case RewardKind.Gold: Gold += r.Amount; break;
                    case RewardKind.Cash: Cash += r.Amount; break;
                    default: _permanentItems.Add(r); break;
                }
            }
            Changed?.Invoke();
        }
    }
}
