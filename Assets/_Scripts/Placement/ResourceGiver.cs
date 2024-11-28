using UnityEngine;

public class ResourceGiver : Building, IOutPort
{
    [Header("Resource Giver")]
    [SerializeField] private float _timeToCollect = 1f;
    [SerializeField] private ResourceBehaviour _resourceToGive;
    private Direction _outDirection = Direction.Right;
    private float _elapsedTime = 0f;

    private readonly IInPort[] r_ports = new IInPort[0];

    public Direction OutDirection { get => _outDirection; set => _outDirection = value; }

    public IInPort[] Ports => r_ports;
    public override bool CanBeRotated => true;
    public override bool CanBeDestroyed => true;

    public void Update()
    {
        var port = r_ports[0];
        if (port == null || !port.CanReceiveResource(_resourceToGive.Type))
            return;

        _elapsedTime += Time.deltaTime;
        if (_elapsedTime > _timeToCollect)
        {
            _elapsedTime %= _timeToCollect;
            r_ports[0].ReceiveResource(Instantiate(
                _resourceToGive, new(port.Position.x, port.Position.y), Quaternion.identity
            ));
        }
    }
}
