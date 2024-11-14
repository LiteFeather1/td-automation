﻿using UnityEngine;

public class PathBehaviour : MonoBehaviour, IPath
{
    [SerializeField] private SegmentBehaviour[] _segments;

    public bool ReachedSegmentEnd(int segmentIndex, int pointIndex)
        => pointIndex == _segments[segmentIndex].Points.Count;

    public bool ReachedPathEnd(int segmentIndex, int pointIndex)
        => segmentIndex == pointIndex && ReachedSegmentEnd(segmentIndex, pointIndex);

    public Vector2 GetPoint(int segmentIndex, int pointIndex) 
        => _segments[segmentIndex].Points[pointIndex];
}