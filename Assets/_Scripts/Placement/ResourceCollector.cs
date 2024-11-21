using UnityEngine;

public class ResourceCollector : Building, IOutput
{
    [SerializeField] private float _timeToCollect = 1f;
    [SerializeField] private ResourceBehaviour _giveObject;
    private Direction _outDirection = Direction.Right;
    private float _elapsedTime = 0f;

    public IInPut Input { get; set; }
    public Direction OutDirection { get => _outDirection; set => _outDirection = value; }

    public override bool CanBeRotated => true;

    public void Update()
    {
        if (Input == null || !Input.CanReceiveResource)
            return;

        _elapsedTime += Time.deltaTime;
        if (_elapsedTime > _timeToCollect)
        {
            _elapsedTime %= _timeToCollect;
            Input.GiveResource(Instantiate(
                _giveObject, new(Input.Position.x, Input.Position.y), Quaternion.identity
            ));
        }
    }
}