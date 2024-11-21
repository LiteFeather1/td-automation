using UnityEngine;

public class BeltPath : Building, IOutPort, IInPort
{
    [SerializeField] private float _speed = 5f;

    private ResourceBehaviour _resource;

    public IInPort NextPort { get; set; }

    public Direction InDirection { get; set; } = Direction.Any;

    public Direction OutDirection { get; set; } = Direction.Right;

    public bool CanReceiveResource => _resource == null;

    public override bool CanBeRotated => true;

    public void Update()
    {
        if (NextPort == null || _resource == null || !NextPort.CanReceiveResource)
            return;

        _resource.transform.position = Vector2.MoveTowards(
            _resource.transform.position, NextPort.Position, _speed * Time.deltaTime
        );

        if (Vector2.Distance(_resource.transform.position, NextPort.Position) < float.Epsilon)
        {
            NextPort.GiveResource(_resource);
            _resource = null;
        }
    }

    public void GiveResource(ResourceBehaviour resource)
    {
        _resource = resource;
    }
}
