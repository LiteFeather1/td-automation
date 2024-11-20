using UnityEngine;

public abstract class Building : MonoBehaviour, IPlaceable
{
    [SerializeField] protected SpriteRenderer _spriteRenderer;
    [SerializeField] protected Direction _outDirection = Direction.None;

    public Vector2Int Position { get; set; }

    public abstract bool CanBeRotated { get; }

    public SpriteRenderer SpriteRenderer => _spriteRenderer;

    public Direction OutDirection => _outDirection;

    public abstract Sprite GetSprite(PlacementSystem ps);

    public void SetRotation(Direction outDirection)
    {
        _outDirection = outDirection;
    }
}
