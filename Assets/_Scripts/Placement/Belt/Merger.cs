using UnityEngine;

public class Merger : InPort, IOutPort
{
    private IInPort _port;

    public Direction OutDirection { get; set; } = Direction.Right;

    public override bool CanBeRotated => true;
    public override bool CanBeDestroyed => true;

    internal void Awake() => InDirection = Direction.Any;

    internal void OnDisable()
    {
        if (_port != null)
            _port.OnDestroyed -= PortDestroyed;
    }

    public override void ResourceCentralized()
    {
        if (_port == null || !_port.CanReceiveResource(_resource.Type))
            return;

        _port.ReceiveResource(_resource);
        _resource = null;
    }

    public IInPort GetPort(int _) => _port;

    public void SetPort(IInPort port)
    {
        _port = port;
        _port.OnDestroyed += PortDestroyed;
    }

    private void PortDestroyed(Vector2Int _)
    {
        _port.OnDestroyed -= PortDestroyed;
        _port = null;
    }
}
