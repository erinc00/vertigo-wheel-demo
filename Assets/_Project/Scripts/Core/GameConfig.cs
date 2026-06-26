using UnityEngine;

namespace Vertigo.Core
{
    [CreateAssetMenu(menuName = "Vertigo/Game Config", fileName = "game_config")]
    public sealed class GameConfig : ScriptableObject
    {
        [Header("Spin")]
        [SerializeField, Min(0.1f)] private float spinDuration = 3.5f;
        [SerializeField, Min(1)] private int spinExtraTurns = 5;

        [Header("Reward popup")]
        [SerializeField, Min(0f)] private float rewardDisplaySeconds = 1.1f;

        [Header("Economy")]
        [SerializeField, Min(0)] private long startGold = 0;
        [SerializeField, Min(0)] private long startCash = 0;
        [SerializeField, Min(0)] private long reviveCost = 25;

        public float SpinDuration => spinDuration;
        public int SpinExtraTurns => spinExtraTurns;
        public float RewardDisplaySeconds => rewardDisplaySeconds;
        public long StartGold => startGold;
        public long StartCash => startCash;
        public long ReviveCost => reviveCost;
    }
}
