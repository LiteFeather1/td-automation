using System.Collections.Generic;
using UnityEngine;

public class SegmentBehaviour : MonoBehaviour, ISegment
{
    [SerializeField] private Transform[] _points;
    private Segment _segment;

    public List<Vector2> Points => _segment.Points;

    public void Awake()
    {
        _segment = new(_points.Length);

        foreach (var point in _points)
        {
            _segment.Points.Add(point.position);
        }
    }
}
