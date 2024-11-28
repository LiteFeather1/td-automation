using System;
using UnityEngine;
using LTF.SerializedDictionary;

public abstract class Building : MonoBehaviour, IPlaceable, IHoverable
{
    [Header("Building")]
    [SerializeField] private string _name;
    [SerializeField] private SerializedDictionary<ResourceType, int> _resourcesCost;
    [SerializeField] protected SpriteRenderer _sr;

    public Action<Vector2Int> OnDestroyed { get; set; }
    public Vector2Int Position { get; set; }

    public abstract bool CanBeRotated { get; }
    public abstract bool CanBeDestroyed { get; }

    public string Name => _name;
    public SerializedDictionary<ResourceType, int> ResourceCost => _resourcesCost;

    public void SetColour(Color colour) => _sr.color = colour;
    public void SetSortingOrder(int order) => _sr.sortingOrder = order;

    public virtual void Place() {}

    public virtual void Destroy()
    {
        OnDestroyed?.Invoke(Position);
        Destroy(gameObject);
    }

    public string GetText() => _name;

    public virtual void Hover() { }

    public virtual void Unhover() {}
}
