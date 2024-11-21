using System;
using UnityEngine;

public class ResourceNode : MonoBehaviour, IPlaceable
{
    [SerializeField] private Vector2Int _position;
    [SerializeField] private ResourceBehaviour _resourceToGive;
    [SerializeField] private int _timesThatCanCollect = 256;

    public ResourceType Type => _resourceToGive.Type;

    public Action<ResourceNode> OnDepleted { get; set; }

    public Vector2Int Position { get => _position; set => _position = value; }

    public bool CanBeRotated => false;
    public bool CanBeDestroyed => false;

    public ResourceType GetResource()
    {
        RemoveTime();
        return _resourceToGive.Type;
    }

    public ResourceBehaviour CollectResource()
    {
        RemoveTime();
        return _resourceToGive;
    }

    private void RemoveTime()
    {
        _timesThatCanBeCollect--;
        if (_timesThatCanBeCollect == 0)
        {
            OnDepleted?.Invoke(this);
            Destroy(gameObject);
        }
    }
}
