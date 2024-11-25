using UnityEngine;

public class ResourceProcessor : Building, IInPort, IOutPort
{
    [Header("Resource Processor")]
    [SerializeField] private ResourceType _resourceType;
    [SerializeField] private ResourceBehaviour _processedBehaviourPrefab;
    [SerializeField] private float _timeToProcessResource = 1f;
    private float _elapsedTime;

    private ResourceBehaviour _processingResource;
    private ResourceBehaviour _processedResource;

    public IInPort Port { get; set; }
    public Direction InDirection { get; set; } = Direction.Left;
    public Direction OutDirection { get; set; } = Direction.Right;

    public override bool CanBeRotated => true;
    public override bool CanBeDestroyed => true;

    internal void Update()
    {
        if (
            Port != null
            && _processedResource != null
            && Port.CanReceiveResource(_processedResource.Type)
        )
        {
            Port.ReceiveResource(_processedResource);
            _processedResource = null;
        }

        if (_processingResource == null || _processedResource != null)
            return;

        _elapsedTime += Time.deltaTime;
        if (_elapsedTime < _timeToProcessResource)
            return;

        _elapsedTime %= _timeToProcessResource;

        Destroy(_processingResource.gameObject);
        _processedResource = Instantiate(
            _processedBehaviourPrefab, transform.position, Quaternion.identity
        );
    }

    public bool CanReceiveResource(ResourceType type)
        => _processingResource == null && _processedResource == null && type == _resourceType;

    public void ReceiveResource(ResourceBehaviour resource) => _processingResource = resource;

    public override void Destroy()
    {
        base.Destroy();
        if (_processedResource != null)
            Destroy(_processedResource.gameObject);

        if (_processingResource != null)
            Destroy(_processingResource.gameObject);
    }
}
