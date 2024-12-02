using UnityEngine;

public class BeltPath : InPort, IOutPort
{
    private IInPort _port;

    public Direction OutDirection { get; set; } = Direction.Right;

    public override void ResourceCentralized()
    {
        if (_port == null || !_port.CanReceiveResource(_resource.Type))
            return;

        _port.ReceiveResource(_resource);
        _resource = null;
    }

    internal void OnDisable()
    {
        if (_port != null)
            _port.OnDestroyed -= PortDestroyed;
    }

    public void SetPort(IInPort inPort)
    {
        _port = inPort;
        _port.OnDestroyed += PortDestroyed;
    }

    private void PortDestroyed(Vector2Int _)
    {
        _port.OnDestroyed -= OnDestroyed;
        _port = null;
    }
}
