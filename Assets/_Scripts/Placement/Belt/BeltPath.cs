using UnityEngine;

public class BeltPath : InPort, IOutPort
{

    [Header("Belt Path")]
    [SerializeField] private Transform _arrow;

    private IInPort _port;

    public Direction OutDirection { get; set; } = Direction.Right;

    private void OnDisable()
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

    public void SetPort(IInPort inPort)
    {
        _port = inPort;
        _port.OnDestroyed += PortDestroyed;
    }

    public void SetArrowRotation(float angle)
    {
        _arrow.transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void PortDestroyed(Vector2Int _)
    {
        _port.OnDestroyed -= OnDestroyed;
        _port = null;
    }
}
