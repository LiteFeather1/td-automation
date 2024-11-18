using UnityEngine;

public abstract class Building : MonoBehaviour, IPlaceable
{
    [SerializeField] protected SpriteRenderer _spriteRenderer;

    public abstract bool CanBeRotated { get; }

    public abstract Sprite GetSprite(PlacementSystem ps);

    public SpriteRenderer SpriteRenderer => _spriteRenderer;
}
