using UnityEngine;

public interface IPlaceable
{
    public Vector2 Position { get; set; }

    public Direction InDirection { get; }
    public Direction OutDirection { get; }

    public bool CanBeRotated { get; }

    public SpriteRenderer SpriteRenderer { get; }

    public Sprite GetSprite(PlacementSystem ps);

    public void SetRotations(Direction inDirection, Direction outDirection);
}
