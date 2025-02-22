﻿using System.Collections.Generic;
using UnityEngine;

public class SegmentBehaviour : MonoBehaviour, ISegment
{
    [SerializeField] private Transform[] _points = new Transform[0];
    private Segment _segment;

    public List<Vector2> Points => _segment.Points;

    public void Awake()
    {
        _segment = new(_points.Length);

        foreach (Transform point in _points)
        {
            _segment.Points.Add(point.position);
        }
    }

#if UNITY_EDITOR
    public void OnDrawGizmosSelected()
    {
        if (_points.Length == 0)
            return;

        int length = _points.Length - 1;
        for (int i = 0; i < length; i++)
        {
            if (_points[i] == null || _points[i + 1] == null)
                continue;

            Gizmos.color = Color.yellow;
            Vector2 midPoint = (_points[i].position + _points[i + 1].position) * .5f;
            Gizmos.DrawLine(_points[i].position, midPoint);
            Gizmos.color = Color.white;
            Gizmos.DrawLine(midPoint, _points[i + 1].position);

            DrawPoint(_points[i], Color.cyan);
        }

        DrawPoint(_points[_points.Length - 1], Color.red);

        static void DrawPoint(Transform point, Color pointColour)
        {
            Gizmos.color = pointColour;
            Gizmos.DrawSphere(point.position, .666f);
        }
    }
#endif
}
