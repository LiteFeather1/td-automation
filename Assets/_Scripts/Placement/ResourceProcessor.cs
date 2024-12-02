using UnityEngine;

public class ResourceProcessor : InPort, IOutPort
{
    [Header("Resource Processor")]
    [SerializeField] private ResourceType _resourceType;
    [SerializeField] private SOObjectPoolResourceBehaviour _processedBehaviourPool;
    [SerializeField] private float _timeToProcessResource = 1f;
    private float _elapsedTime;

    private bool _hasProcessedResource;

    private IInPort _port;

    public Direction OutDirection { get; set; } = Direction.Right;

    internal void OnDisable()
    {
        if (_port != null)
            _port.OnDestroyed -= PortDestroyed;
    }

    public override bool CanReceiveResource(ResourceType type)
        => base.CanReceiveResource(type) && !_hasProcessedResource && type == _resourceType;

    public override void ResourceCentralized()
    {
        if (_hasProcessedResource)
        {
            if (_port != null && _port.CanReceiveResource(_resource.Type))
            {
                _port.ReceiveResource(_resource);
                _resource = null;
                _hasProcessedResource = false;
            }

            return;
        }

        _elapsedTime += Time.deltaTime;
        if (_elapsedTime < _timeToProcessResource)
            return;

        _elapsedTime %= _timeToProcessResource;

        _resource.Deactive();
        _resource = _processedBehaviourPool.ObjectPool.GetObject();
        _resource.transform.position = transform.position;
        _resource.gameObject.SetActive(true);
        _hasProcessedResource = true;
    }

    public void SetPort(IInPort inPort)
    {
        _port = inPort;
        _port.OnDestroyed += PortDestroyed;
    }

    private void PortDestroyed(Vector2Int _)
    {
        _port.OnDestroyed -= PortDestroyed;
        _port = null;
    }
}
