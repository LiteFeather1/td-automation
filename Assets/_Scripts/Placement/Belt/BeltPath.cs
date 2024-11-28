using UnityEngine;

public class BeltPath : Building, IOutPort, IInPort
{
    [Header("Belt Path")]
    [SerializeField] private float _itemMoveSpeed = 2f;
    private ResourceBehaviour _resource;

    public bool debug;

    private readonly IInPort[] r_ports = new IInPort[1];

    public Direction InDirection { get; set; } = Direction.Left;
 
    public Direction OutDirection { get; set; } = Direction.Right;

    public IInPort[] Ports => r_ports;

    public override bool CanBeRotated => true;
    public override bool CanBeDestroyed => true;

    public void Update()
    {
        var port = r_ports[0];
        if (debug)
            print(port);
        if (port == null || _resource == null || !port.CanReceiveResource(_resource.Type))
            return;

        _resource.transform.position = Vector2.MoveTowards(
            _resource.transform.position, port.Position, _itemMoveSpeed * Time.deltaTime
        );

        if (Vector2.Distance(_resource.transform.position, port.Position) < float.Epsilon)
        {
            port.ReceiveResource(_resource);
            _resource = null;
        }
    }

    public bool CanReceiveResource(ResourceType _) => _resource == null;

    public void ReceiveResource(ResourceBehaviour resource)
    {
        _resource = resource;
    }

    public override void Destroy()
    {
        _resource?.Deactive();
        base.Destroy();
    }
}
