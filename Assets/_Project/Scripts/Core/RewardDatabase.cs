using System.Collections.Generic;
using UnityEngine;

namespace Vertigo.Core
{
    [CreateAssetMenu(menuName = "Vertigo/Reward Database", fileName = "reward_database")]
    public sealed class RewardDatabase : ScriptableObject
    {
        [SerializeField] private List<RewardData> rewards = new();

        private Dictionary<string, RewardData> _lookup;

        public RewardData GetById(string id)
        {
            if (_lookup == null)
            {
                _lookup = new Dictionary<string, RewardData>();
                for (int i = 0; i < rewards.Count; i++)
                {
                    if (rewards[i] != null && !_lookup.ContainsKey(rewards[i].Id))
                        _lookup[rewards[i].Id] = rewards[i];
                }
            }
            return _lookup.TryGetValue(id, out var r) ? r : null;
        }
    }
}