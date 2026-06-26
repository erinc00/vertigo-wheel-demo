using UnityEngine;

namespace Vertigo.Core
{
    [CreateAssetMenu(menuName = "Vertigo/Reward", fileName = "reward_")]
    public sealed class RewardData : ScriptableObject
    {
        [SerializeField] private string id = "reward_id";
        [SerializeField] private RewardKind kind = RewardKind.Gold;
        [SerializeField] private string displayName = "Reward";
        [SerializeField, Min(0)] private long baseAmount = 100;
        [SerializeField] private Sprite icon;
        [Tooltip("Aynı id'li ödüller toplanır mı (para/puan = true, eşsiz skin = false)")]
        [SerializeField] private bool stackable = true;

        public string Id => id;
        public RewardKind Kind => kind;
        public string DisplayName => displayName;
        public long BaseAmount => baseAmount;
        public Sprite Icon => icon;
        public bool Stackable => stackable;
    }
}
