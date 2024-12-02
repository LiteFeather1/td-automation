using UnityEngine;

public class Splitter : InPort, IOutPort
{
    private readonly IInPort[] r_ports = new IInPort[3];

    private int _offset;

    public Direction OutDirection { get; set; } = Direction.Any;

    public override bool CanBeRotated => true;
    public override bool CanBeDestroyed => true;

    internal void OnDisable()
    {
        foreach (var port in r_ports)
            if (port != null)
                port.OnDestroyed -= PortDestroyed;
    }

    public override void ResourceCentralized()
    {
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

    public void SetPort(IInPort inPort)
    {
        for (var i = 0; i < r_ports.Length; i++)
        {
            if (r_ports[i] == null)
            {
                print(inPort);
                r_ports[i] = inPort;
                inPort.OnDestroyed += PortDestroyed;
                break;
            }
        }
    }

    private void PortDestroyed(Vector2Int position)
    {
        for (var i = 0; i < r_ports.Length; i++)
        {
            if (r_ports[i] != null && r_ports[i].Position == position)
            {
                r_ports[i].OnDestroyed -= PortDestroyed;
                r_ports[i] = null;
                break;
            }
        }
    }
}
