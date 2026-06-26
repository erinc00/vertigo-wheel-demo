using UnityEngine;

namespace Vertigo.Core
{
    public sealed class RewardInstance
    {
        public string Id { get; }
        public RewardKind Kind { get; }
        public string DisplayName { get; }
        public long Amount { get; private set; }
        public Sprite Icon { get; }
        public bool Stackable { get; }

        public RewardInstance(RewardData data, long amount)
        {
            Id = data.Id;
            Kind = data.Kind;
            DisplayName = data.DisplayName;
            Amount = amount;
            Icon = data.Icon;
            Stackable = data.Stackable;
        }

        public void Add(long extra) => Amount += extra;
    }
}
