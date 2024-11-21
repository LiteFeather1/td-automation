using UnityEngine;

public class ResourceGiver : Building, IOutPort
{
    [SerializeField] private float _timeToCollect = 1f;
    [SerializeField] private ResourceBehaviour _resourceToGive;
    private Direction _outDirection = Direction.Right;
    private float _elapsedTime = 0f;

    public IInPort Port { get; set; }
    public Direction OutDirection { get => _outDirection; set => _outDirection = value; }

    public override bool CanBeRotated => true;
    public override bool CanBeDestroyed => true;

    public void Update()
    {
        if (Port == null || !Port.CanReceiveResource)
            return;

        _elapsedTime += Time.deltaTime;
        if (_elapsedTime > _timeToCollect)
        {
            _elapsedTime %= _timeToCollect;
            Port.GiveResource(Instantiate(
                _resourceToGive, new(Port.Position.x, Port.Position.y), Quaternion.identity
            ));
        }
    }
}
