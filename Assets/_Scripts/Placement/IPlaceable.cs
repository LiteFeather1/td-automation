using UnityEngine;

public interface IPlaceable
{
    public Direction InDirection { get; }
    public Direction OutDirection { get; }

    public bool CanBeRotated { get; }

    public SpriteRenderer SpriteRenderer { get; }

    public Sprite GetSprite(PlacementSystem ps);
}
