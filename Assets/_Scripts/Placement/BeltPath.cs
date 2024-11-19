using UnityEngine;

public class BeltPath : Building, IPlaceable
{
    public override bool CanBeRotated { get; } = true;

    public override Sprite GetSprite(PlacementSystem ps)
    {
        return _spriteRenderer.sprite;
    }
}
