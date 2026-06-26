using System;
using System.Collections.Generic;
using UnityEngine;

namespace Vertigo.Core
{
    [Serializable]
    public sealed class RewardPoolEntry
    {
        [SerializeField] private RewardData reward;
        [SerializeField, Min(0)] private int weight = 1;

        public RewardData Reward => reward;
        public int Weight => weight;
    }

    [CreateAssetMenu(menuName = "Vertigo/Reward Pool", fileName = "pool_")]
    public sealed class RewardPool : ScriptableObject
    {
        [SerializeField] private List<RewardPoolEntry> entries = new();

        public RewardData PickRandom(IRandomProvider random)
        {
            int total = 0;
            for (int i = 0; i < entries.Count; i++)
                total += Mathf.Max(0, entries[i].Weight);

            if (total <= 0)
                return entries.Count > 0 ? entries[0].Reward : null;

            int roll = random.Next(total);
            int acc = 0;
            for (int i = 0; i < entries.Count; i++)
            {
                acc += Mathf.Max(0, entries[i].Weight);
                if (roll < acc) return entries[i].Reward;
            }
            return entries[entries.Count - 1].Reward;
        }
    }
}