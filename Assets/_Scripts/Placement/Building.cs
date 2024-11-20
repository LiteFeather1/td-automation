using UnityEngine;

public abstract class Building : MonoBehaviour, IPlaceable
{
    [SerializeField] protected SpriteRenderer _spriteRenderer;
    [SerializeField] protected Direction _inDirection = Direction.None;
    [SerializeField] protected Direction _outDirection = Direction.None;

    public Vector2 Position { get; set; }

    public abstract bool CanBeRotated { get; }

    public SpriteRenderer SpriteRenderer => _spriteRenderer;

    public Direction InDirection => _inDirection;
    public Direction OutDirection => _outDirection;

    public abstract Sprite GetSprite(PlacementSystem ps);

    public void SetRotations(Direction inDirection, Direction outDirection)
    {
        _inDirection = inDirection;
        _outDirection = outDirection;
    }
}
