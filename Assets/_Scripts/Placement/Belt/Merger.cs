using UnityEngine;

public class Merger : Building, IOutPort, IInPort
{
    private IInPort _port;
    private int _offset;

    private ResourceBehaviour _resource;

    public Direction OutDirection { get; set; } = Direction.Right;
    public Direction InDirection { get; set; } = Direction.Any;

    public override bool CanBeRotated => true;
    public override bool CanBeDestroyed => true;

    internal void Update()
    {
        if (_resource == null || _port == null || !_port.CanReceiveResource(_resource.Type))
            return;

        _port.ReceiveResource(_resource);
        _resource.transform.position = (Vector2)_port.Position;
        _resource = null;
    }

    internal void OnDisable()
    {
        if (_port != null)
            _port.OnDestroyed -= PortDestroyed;
    }

    public IInPort GetPort(int _) => _port;

    public void SetPort(IInPort port, int _)
    {
        _port = port;
        _port.OnDestroyed += PortDestroyed;
    }

    public bool CanReceiveResource(ResourceType _) => _resource == null;

    public void ReceiveResource(ResourceBehaviour resource)
    {
        _resource = resource;
    }

    public override void Destroy()
    {
        base.Destroy();
        _resource?.Deactive();
    }

    private void PortDestroyed(Vector2Int _)
    {
        _port.OnDestroyed -= PortDestroyed;
        _port = null;
    }
}
