using UnityEngine;

public static class Helpers
{
    public static float Map(float value, float min1, float max1, float min2, float max2)
    {
        float t = (value - min1) / (max1 - min1);
        return (1f - t) * min2 + max2 * t;
    }

    // From https://easings.net/#easeInOutQuad
    public static float EaseInOutQuad(float x)
        => (x < 0.5f) ? (2f * x * x) : (1f - Mathf.Pow(-2f * x + 2f, 2f) * .5f);

    // https://easings.net/#easeInOutCubic
    public static float EaseInOutCubic(float f)
        => f < 0.5f ? 4f * f * f * f : 1f - Mathf.Pow(-2f * f + 2f, 3f) / 2f;
}
