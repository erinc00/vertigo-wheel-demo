using System;
using UnityEngine;

namespace Vertigo.Core
{
    [Serializable]
    public sealed class WheelSliceData
    {
        [SerializeField] private RewardData reward;
        [SerializeField] private RewardPool rewardPool;
        [SerializeField] private bool isBomb;
        [SerializeField] private Sprite iconOverride;
        [SerializeField, Min(0)] private int weight = 1;
        [SerializeField] private string displayLabel;

        public RewardData Reward => reward;
        public RewardPool Pool => rewardPool;
        public bool IsBomb => isBomb;
        public int Weight => weight;
        public string DisplayLabel => displayLabel;

        public Sprite ResolveIcon(RewardData effectiveReward)
        {
            if (iconOverride != null) return iconOverride;
            return effectiveReward != null ? effectiveReward.Icon : null;
        }
    }
}