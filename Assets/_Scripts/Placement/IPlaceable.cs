using UnityEngine;

public interface IPlaceable
{
    public bool CanBeRotated { get; }

    public SpriteRenderer SpriteRenderer { get; }

    public Sprite GetSprite(PlacementSystem ps);
}
