using System;
using UnityEngine;

public class ResourceNode : MonoBehaviour, IPlaceable
{
    [SerializeField] private ResourceBehaviour _resourceToGive;
    [SerializeField] private int _timesThatCanCollect = 256;

    public ResourceType Type => _resourceToGive.Type;

    public Action<ResourceNode> OnDepleted { get; set; }

    public Vector2Int Position { get; set; }

    public bool CanBeRotated => throw new NotImplementedException();

    public bool CanBeDestroyed => throw new NotImplementedException();

    public void Awake()
    {
        Position = Vector2Int.RoundToInt(transform.position);
    }

    public ResourceBehaviour CollectResource()
    {
        _timesThatCanCollect--;
        if (_timesThatCanCollect == 0)
        {
            OnDepleted?.Invoke(this);
            Destroy(gameObject);
        }

        return _resourceToGive;
    }
}
