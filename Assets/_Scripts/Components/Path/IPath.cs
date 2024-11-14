using UnityEngine;

public interface IPath
{
    public bool ReachedSegmentEnd(int segmentIndex, int pointIndex);
    public bool ReachedPathEnd(int segmentIndex);
    public Vector2 GetPoint(int segmentIndex, int pointIndex);
}
