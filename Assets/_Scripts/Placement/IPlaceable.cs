using UnityEngine;

public interface IPlaceable
{
    public Vector2Int Position { get; set; }

    public Direction OutDirection { get; }

    public bool CanBeRotated { get; }

    public SpriteRenderer SpriteRenderer { get; }

    public Sprite GetSprite(PlacementSystem ps);

    public void SetRotation(Direction outDirection);
}
