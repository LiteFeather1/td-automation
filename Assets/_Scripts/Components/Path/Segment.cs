using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Segment : ISegment
{
    private List<Vector2> _points;

    public List<Vector2> Points => _points;

    public Segment() => _points = new();
    public Segment(int length) => _points = new(length);
}
