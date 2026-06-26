namespace Vertigo.Core
{
    public enum ZoneType
    {
        Normal,
        Safe,
        Super
    }

    public enum GameState
    {
        PreparingZone,
        WaitingForSpin,
        Spinning,
        ShowingReward,
        BombHit,
        RunCompleted
    }

    public enum RewardKind
    {
        Gold,
        Cash,
        Chest,
        WeaponPoints,
        Skin,
        Consumable,
        Cosmetic
    }
}
