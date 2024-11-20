using UnityEngine;

public class BeltPath : Building, IOutPut
{
    [SerializeField] private float _speed = 5f;
    private Direction _outDirection = Direction.Right;

	public Transform CurrentObject { get; set; }

    public BeltPath OutBelt { get; set; }

    public Direction OutDirection { get => _outDirection; set => _outDirection = value; }

    public override bool CanBeRotated => true;

    public void Update()
    {
        if (OutBelt == null || CurrentObject == null || OutBelt.CurrentObject != null)
            return;

        CurrentObject.position = Vector2.MoveTowards(
            CurrentObject.position, OutBelt.Position, _speed * Time.deltaTime
        );

        if (Vector2.Distance(CurrentObject.position, OutBelt.Position) < float.Epsilon)
        {
            OutBelt.CurrentObject = CurrentObject;
            CurrentObject = null;
        }
    }
}
