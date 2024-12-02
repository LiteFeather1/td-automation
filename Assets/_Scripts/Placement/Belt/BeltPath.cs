using UnityEngine;

public class BeltPath : InPort, IOutPort
{
    [Header("Belt Path")]
    [SerializeField] private float _itemMoveSpeed = 2f;

    private IInPort _port;

    public Direction OutDirection { get; set; } = Direction.Right;

    public void Update()
    {
        if (_resource == null)
            return;

        if (Vector2.Distance(_resource.transform.position, transform.position) > float.Epsilon)
        {
            _resource.transform.position = Vector2.MoveTowards(
                _resource.transform.position, transform.position, _itemMoveSpeed * Time.deltaTime
            );
        }
        else if (_port != null && _port.CanReceiveResource(_resource.Type))
        {
            _port.ReceiveResource(_resource);
            _resource = null;
        }
    }

    internal void OnDisable()
    {
        if (_port != null)
            _port.OnDestroyed -= PortDestroyed;
    }

    public IInPort GetPort(int _) => _port;

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
