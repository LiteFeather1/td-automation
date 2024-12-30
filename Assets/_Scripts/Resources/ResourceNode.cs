using System;
using UnityEngine;

public class ResourceNode : MonoBehaviour, IHoverable
{
    private const float MIN_SIZE = 0.1f;

    private static readonly int sr_size = Shader.PropertyToID("_Size");

    [SerializeField] private string _name;
    [SerializeField] private int _timesThatCanBeCollect = 256;
    private int _collectedTimes;

    [SerializeField] private Vector2Int _position;

    [Space]
    [SerializeField] private SOObjectPoolResourceBehaviour _resourceToGivePool;

    [Space]
    [SerializeField] private SpriteRenderer _sr;

    public ResourceType Type => _resourceToGivePool.ObjectPool.Object.Type;

    public Action<ResourceNode> OnDepleted { get; set; }

    public Vector2Int Position { get => _position; set => _position = value; }

    public bool Depleted => _collectedTimes == _timesThatCanBeCollect;

    public ResourceType GetResource()
    {
        RemoveTime();
        return Type;
    }

    public ResourceBehaviour CollectResource()
    {
        RemoveTime();
        return _resourceToGivePool.ObjectPool.GetObject();
    }

    private void RemoveTime()
    {
        _collectedTimes++;
        _sr.material.SetFloat(sr_size, Mathf.Lerp(
            MIN_SIZE, transform.localScale.y - MIN_SIZE, 1f - _collectedTimes / (float)_timesThatCanBeCollect
        ));

        if (Depleted)
        {
            OnDepleted?.Invoke(this);
            Destroy(gameObject);
        }
    }

    public string GetText()
    {
        return $"{_name}\n{_timesThatCanBeCollect - _collectedTimes:000}";
    }

    public void Hover() { }

    public void Unhover() {}
}
