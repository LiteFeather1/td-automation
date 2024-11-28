using UnityEngine;

public class ResourceProcessor : Building, IInPort, IOutPort
{
    [Header("Resource Processor")]
    [SerializeField] private ResourceType _resourceType;
    [SerializeField] private SOObjectPoolResourceBehaviour _processedBehaviourPool;
    [SerializeField] private float _timeToProcessResource = 1f;
    private float _elapsedTime;

    private readonly IInPort[] r_ports = new IInPort[1];

    private ResourceBehaviour _processingResource;
    private ResourceBehaviour _processedResource;

    public Direction InDirection { get; set; } = Direction.Left;
    public Direction OutDirection { get; set; } = Direction.Right;

    public IInPort[] Ports => r_ports;
    public override bool CanBeRotated => true;
    public override bool CanBeDestroyed => true;

    internal void Update()
    {
        var port = r_ports[0];
        if (
            port != null
            && _processedResource != null
            && port.CanReceiveResource(_processedResource.Type)
        )
        {
            port.ReceiveResource(_processedResource);
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
        _processedResource.transform.position = (Vector2)port.Position;
        _processedResource.gameObject.SetActive(true);
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
}
