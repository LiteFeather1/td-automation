using UnityEngine;

public interface IPlaceable
{
    public Vector2Int Position { get; set; }
    public bool CanBeRotated { get; }
}
