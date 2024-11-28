using System;
using UnityEngine;

public interface IPlaceable
{
    public Action<Vector2Int> OnDestroyed { get; set; }
    public Vector2Int Position { get; set; }
    public bool CanBeRotated { get; }
    public bool CanBeDestroyed { get; }
}
