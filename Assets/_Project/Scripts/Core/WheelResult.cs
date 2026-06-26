namespace Vertigo.Core
{
    public readonly struct WheelResult
    {
        public readonly int SliceIndex;
        public readonly bool IsBomb;
        public readonly RewardData Reward;

        public WheelResult(int sliceIndex, bool isBomb, RewardData reward)
        {
            SliceIndex = sliceIndex;
            IsBomb = isBomb;
            Reward = reward;
        }
    }
}
