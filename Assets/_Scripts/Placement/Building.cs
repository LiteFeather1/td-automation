using System;
using UnityEngine;

public abstract class Building : MonoBehaviour, IPlaceable, IHoverable
{
    [Header("Building")]
    [SerializeField] private PlaceableData _data;
    [SerializeField] protected SpriteRenderer _sr;

    public Action<Vector2Int> OnDestroyed { get; set; }
    public Vector2Int Position { get; set; }

    public abstract bool CanBeRotated { get; }
    public abstract bool CanBeDestroyed { get; }

    public PlaceableData Data => _data;

    public SpriteRenderer SR => _sr;

    public virtual void Place() {}

    public virtual void Destroy()
    {
        OnDestroyed?.Invoke(Position);
        Destroy(gameObject);
    }

    public string GetText() => _data.Name;

    public virtual void Hover() { }

    public virtual void Unhover() { }
}
