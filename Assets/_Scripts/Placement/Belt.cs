using UnityEngine;

public class Belt : Building, IPlaceable
{
    public override bool CanBeRotated { get; } = true;

    public override Sprite GetSprite(PlacementSystem ps)
    {
        return _spriteRenderer.sprite;
    }
}
