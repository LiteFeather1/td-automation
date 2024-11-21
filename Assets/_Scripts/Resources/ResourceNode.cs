using System;
using UnityEngine;

public class ResourceNode : MonoBehaviour
{
    [SerializeField] private ResourceBehaviour _resourceToGive;
    [SerializeField] private int _timesThatCanCollect = 256;

    public ResourceType ResourceType => _resourceToGive.Type;

    public Action<ResourceNode> OnDepleted { get; set; }

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
