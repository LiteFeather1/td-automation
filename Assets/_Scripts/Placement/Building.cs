using UnityEngine;
using LTF.SerializedDictionary;

public abstract class Building : MonoBehaviour, IPlaceable, IHoverable
{
    [Header("Building")]
    [SerializeField] private string _name;
    [SerializeField] private SerializedDictionary<ResourceType, int> _resourcesCost;

    public Vector2Int Position { get; set; }
    public abstract bool CanBeRotated { get; }
    public abstract bool CanBeDestroyed { get; }

    public string Name => _name;
    public SerializedDictionary<ResourceType, int> ResourceCost => _resourcesCost;

    public virtual void Destroy()
    {
        Destroy(gameObject);
    }

    public string GetText() => _name;

    public virtual void Hover() { }

    public void Unhover() {}
}
