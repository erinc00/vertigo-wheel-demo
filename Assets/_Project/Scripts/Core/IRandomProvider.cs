namespace Vertigo.Core
{
    public interface IRandomProvider
    {
        int Next(int maxExclusive);
        float NextFloat(float min, float max);
    }
}
