using UnityEngine;

public class BeltPath : Building, IOutput, IInPut
{
    [SerializeField] private float _speed = 5f;

    private ResourceBehaviour _resource;

    public IInPut Input { get; set; }

    public Direction InDirection { get; set; } = Direction.Any;

    public Direction OutDirection { get; set; } = Direction.Right;

    public bool CanReceiveResource => _resource == null;

    public override bool CanBeRotated => true;

    public void Update()
    {
        if (Input == null || _resource == null || !Input.CanReceiveResource)
            return;

        _resource.transform.position = Vector2.MoveTowards(
            _resource.transform.position, Input.Position, _speed * Time.deltaTime
        );

        if (Vector2.Distance(_resource.transform.position, Input.Position) < float.Epsilon)
        {
            Input.GiveResource(_resource);
            _resource = null;
        }
    }

    public void GiveResource(ResourceBehaviour resource)
    {
        _resource = resource;
    }
}
