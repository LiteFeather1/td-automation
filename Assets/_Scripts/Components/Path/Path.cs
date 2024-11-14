using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Path<T> : IPath where T : ISegment
{
    [SerializeField] private List<T> _segments;

    public List<T> Segments => _segments;

    public bool ReachedSegmentEnd(int segmentIndex, int pointIndex)
        => pointIndex == _segments[segmentIndex].Points.Count;

    public bool ReachedPathEnd(int segmentIndex)
        => segmentIndex == _segments.Count;

    public Vector2 GetPoint(int segmentIndex, int pointIndex) 
        => _segments[segmentIndex].Points[pointIndex];
}
