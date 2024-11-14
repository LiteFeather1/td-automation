﻿using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Path : IPath
{
    [SerializeField] private List<Segment> _segments;

    public bool ReachedSegmentEnd(int segmentIndex, int pointIndex)
        => pointIndex == _segments[segmentIndex].Points.Count;

    public bool ReachedPathEnd(int segmentIndex, int pointIndex)
    => _segments.Count == segmentIndex 
        && ReachedSegmentEnd(segmentIndex, pointIndex);

    public Vector2 GetPoint(int segmentIndex, int pointIndex) 
        => _segments[segmentIndex].Points[pointIndex];
}
