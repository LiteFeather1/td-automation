using UnityEngine;

public class ResourceProcessor : Building, IInPort, IOutPort
{
    [Header("Resource Processor")]
    [SerializeField] private ResourceType _resourceType;
    [SerializeField] private SOObjectPoolResourceBehaviour _processedBehaviourPool;
    [SerializeField] private float _timeToProcessResource = 1f;
    private float _elapsedTime;

    private IInPort _port;

    private ResourceBehaviour _processingResource;
    private ResourceBehaviour _processedResource;

    public Direction InDirection { get; set; } = Direction.Left;
    public Direction OutDirection { get; set; } = Direction.Right;

    public override bool CanBeRotated => true;
    public override bool CanBeDestroyed => true;

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

        if (_processingResource == null || _processedResource != null)
            return;

        _elapsedTime += Time.deltaTime;
        if (_elapsedTime < _timeToProcessResource)
            return;

        _elapsedTime %= _timeToProcessResource;

        _processingResource.Deactive();
        _processedResource = _processedBehaviourPool.ObjectPool.GetObject();
        _processedResource.transform.position = (Vector2)_port.Position;
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

    public bool CanReceiveResource(ResourceType type)
        => _processingResource == null && _processedResource == null && type == _resourceType;

    public void ReceiveResource(ResourceBehaviour resource) => _processingResource = resource;

    public override void Destroy()
    {
        base.Destroy();
        _processedResource?.Deactive();
        _processingResource?.Deactive();
    }

    private void PortDestroyed(Vector2Int _)
    {
        _port.OnDestroyed -= PortDestroyed;
        _port = null;
    }
}
