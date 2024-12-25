
public static class Helpers
{
    public static float Map(float value, float min1, float max1, float min2, float max2)
    {
        float t = (value - min1) / (max1 - min1);
        return (1f - t) * min2 + max2 * t;
    }
}
