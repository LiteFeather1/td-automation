public class Splitter : Building, IOutPort, IInPort
{
    private readonly IInPort[] r_ports = new IInPort[3];

    private int _offset;

    private ResourceBehaviour _resource;

    public Direction OutDirection { get; set; } = Direction.Any;
    public Direction InDirection { get; set; } = Direction.Left;

    public override bool CanBeRotated => true;
    public override bool CanBeDestroyed => true;

    internal void Update()
    {
        if (_resource == null)
            return;

        for (var i = 0; i < r_ports.Length; i++)
        {
            var port = r_ports[(i + _offset) % r_ports.Length];
            if (port != null && port.CanReceiveResource(_resource.Type))
            {
                port.ReceiveResource(_resource);
                _resource = null;
                _offset++;
                break;
            }
        }
    }

    public IInPort GetPort(int index)
    {
        return r_ports[index];
    }

    public void SetPort(IInPort inPort, int index)
    {
        r_ports[index] = inPort;
    }

    public bool CanReceiveResource(ResourceType type)
    {
        return _resource == null;
    }

    public void ReceiveResource(ResourceBehaviour resource)
    {
        _resource = resource;
    }

    public override void Destroy()
    {
        base.Destroy();
        _resource?.Deactive();
    }
}
