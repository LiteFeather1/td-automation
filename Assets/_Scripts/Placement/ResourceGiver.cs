using System;
using UnityEngine;

public class ResourceGiver : Building, IOutPort
{
    [Header("Resource Giver")]
    [SerializeField] private float _timeToCollect = 1f;
    [SerializeField] private ResourceBehaviour _resourceToGive;
    private Direction _outDirection = Direction.Right;
    private float _elapsedTime = 0f;

    private IInPort _port;

    public Direction OutDirection { get => _outDirection; set => _outDirection = value; }

    public override bool CanBeRotated => true;
    public override bool CanBeDestroyed => true;

    public void Update()
    {
        if (_port == null || !_port.CanReceiveResource(_resourceToGive.Type))
            return;

        _elapsedTime += Time.deltaTime;
        if (_elapsedTime > _timeToCollect)
        {
            _elapsedTime %= _timeToCollect;
            _port.ReceiveResource(Instantiate(
                _resourceToGive, new(_port.Position.x, _port.Position.y), Quaternion.identity
            ));
        }
    }

    internal void OnDisable()
    {
        if (_port != null)
            _port.OnDestroyed -= PortDestroyed;
    }

    public void SetPort(IInPort inPort)
    {
        _port = inPort;
        inPort.OnDestroyed += PortDestroyed;
    }

    private void PortDestroyed(Vector2Int _)
    {
        _port.OnDestroyed -= PortDestroyed;
        _port = null;
    }
}
