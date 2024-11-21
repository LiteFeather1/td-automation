using UnityEngine;
using LTF.SerializedDictionary;

public abstract class Building : MonoBehaviour, IPlaceable
{
    [SerializeField] private SerializedDictionary<ResourceType, int> _resourcesCost;

    public Vector2Int Position { get; set; }
    public abstract bool CanBeRotated { get; }
    public abstract bool CanBeDestroyed { get; }

    public SerializedDictionary<ResourceType, int> ResourceCost => _resourcesCost;

    public virtual void Destroy()
    {
        Destroy(gameObject);
    }
}
