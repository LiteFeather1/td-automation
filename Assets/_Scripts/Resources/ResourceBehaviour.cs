using System;
using UnityEngine;

public class ResourceBehaviour : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _sr;
    [SerializeField] private ResourceType _resourceType;

    public Action<ResourceBehaviour> OnReturnToPool { get; set; }

    public ResourceType Type => _resourceType;

    public void SetSprite(Sprite sprite) => _sr.sprite = sprite;

    public void SetResourceType(ResourceType resourceType) => _resourceType = resourceType;

    public void Deactive()
    {
        OnReturnToPool?.Invoke(this);
        gameObject.SetActive(false);
    }
}
