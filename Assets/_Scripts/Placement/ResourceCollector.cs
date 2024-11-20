using UnityEngine;

public class ResourceCollector : Building, IOutPut
{
    [SerializeField] private float _timeToCollect = 1f;
    [SerializeField] private Transform _giveObject;
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
            var newObject = Instantiate(_giveObject, (Vector3Int)OutBelt.Position, Quaternion.identity);
            OutBelt.CurrentObject = newObject;
        }
    }
}