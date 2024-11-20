using UnityEngine;

public class ResourceCollector : Building, IOutPut
{
    [SerializeField] private float _timeToCollect = 1f;
    [SerializeField] private ResourceBehaviour _giveObject;
    private Direction _outDirection = Direction.Right;
    private float _elapsedTime = 0f;

    public BeltPath OutBelt { get; set; }
    public Direction OutDirection { get => _outDirection; set => _outDirection = value; }

    public override bool CanBeRotated => true;

    public void Update()
    {
        if (OutBelt == null || OutBelt.CurrentObject != null)
            return;

        _elapsedTime += Time.deltaTime;
        if (_elapsedTime > _timeToCollect)
        {
            _elapsedTime %= _timeToCollect;
            OutBelt.CurrentObject = Instantiate(
                _giveObject, new(OutBelt.Position.x, OutBelt.Position.y), Quaternion.identity
            );
        }
    }
}