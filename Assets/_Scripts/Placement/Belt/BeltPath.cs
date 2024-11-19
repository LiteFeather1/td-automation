using UnityEngine;

public class BeltPath : Building, IPlaceable
{
    public BeltPath InBelt { get; set; }
    public BeltPath OutBelt { get; set; }

    public override bool CanBeRotated { get; } = true;

    public override Sprite GetSprite(PlacementSystem ps)
    {
        return _spriteRenderer.sprite;
    }
}
