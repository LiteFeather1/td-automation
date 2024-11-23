using UnityEngine;

public class BeltPath : Building, IOutPort, IInPort
{
    [Header("Belt Path")]
    [SerializeField] private float _itemMoveSpeed = 2f;

    private ResourceBehaviour _resource;

    public IInPort Port { get; set; }

    public Direction InDirection { get; set; } = Direction.Left;

    public Direction OutDirection { get; set; } = Direction.Right;

    public bool CanReceiveResource => _resource == null;

    public override bool CanBeRotated => true;
    public override bool CanBeDestroyed => true;

    public void Update()
    {
        if (Port == null || _resource == null || !Port.CanReceiveResource)
            return;

        _resource.transform.position = Vector2.MoveTowards(
            _resource.transform.position, Port.Position, _itemMoveSpeed * Time.deltaTime
        );

        if (Vector2.Distance(_resource.transform.position, Port.Position) < float.Epsilon)
        {
            Port.GiveResource(_resource);
            _resource = null;
        }
    }

    public void GiveResource(ResourceBehaviour resource)
    {
        _resource = resource;
    }

    public override void Destroy()
    {
        if (_resource != null)
            Destroy(_resource.gameObject);

        base.Destroy();
    }
}
