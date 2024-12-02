using UnityEngine;

public class Merger : InPort, IOutPort
{
    private IInPort _port;

    public override Direction InDirection { get; set; } = Direction.Any;

    public Direction OutDirection { get; set; } = Direction.Right;

    public override bool CanBeRotated => true;
    public override bool CanBeDestroyed => true;

    internal void Update()
    {
        if (_resource == null || _port == null || !_port.CanReceiveResource(_resource.Type))
            return;

        _port.ReceiveResource(_resource);
        _resource = null;
    }

    internal void OnDisable()
    {
        if (_port != null)
            _port.OnDestroyed -= PortDestroyed;
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
