using UnityEngine;

public class ResourceProcessor : InPort, IOutPort
{
    [Header("Resource Processor")]
    [SerializeField] private ResourceType _resourceType;
    [SerializeField] private SOObjectPoolResourceBehaviour _processedBehaviourPool;
    [SerializeField] private float _timeToProcessResource = 1f;
    private float _elapsedTime;

    private IInPort _port;

    private ResourceBehaviour _processedResource;

    public Direction OutDirection { get; set; } = Direction.Right;

    internal void Update()
    {;
        if (
            _port != null
            && _processedResource != null
            && _port.CanReceiveResource(_processedResource.Type)
        )
        {
            _port.ReceiveResource(_processedResource);
            _processedResource = null;
        }

        if (_resource == null || _processedResource != null)
            return;

        _elapsedTime += Time.deltaTime;
        if (_elapsedTime < _timeToProcessResource)
            return;

        _elapsedTime %= _timeToProcessResource;

        _resource.Deactive();
        _processedResource = _processedBehaviourPool.ObjectPool.GetObject();
        _processedResource.gameObject.SetActive(true);
    }

    internal void OnDisable()
    {
        if (_port != null)
            _port.OnDestroyed -= PortDestroyed;
    }

    public IInPort GetPort(int _) => _port;

    public void SetPort(IInPort inPort)
    {
        _port = inPort;
        _port.OnDestroyed += PortDestroyed;
    }

    public override bool CanReceiveResource(ResourceType type)
        => base.CanReceiveResource(type) && _processedResource == null && type == _resourceType;

    public override void Destroy()
    {
        _processedResource?.Deactive();
        base.Destroy();
    }

    private void PortDestroyed(Vector2Int _)
    {
        _port.OnDestroyed -= PortDestroyed;
        _port = null;
    }
}
