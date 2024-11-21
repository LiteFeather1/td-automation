using System;
using UnityEngine;

public class Receiver : Building, IInPut
{
    [SerializeField] private Direction _direction = Direction.Any;

    public Direction InDirection { get => _direction; set => _direction = value; }

    public bool CanReceiveResource => true;

    public override bool CanBeRotated => true;

    public Action<ResourceBehaviour> OnResourceGot { get; set; }

    public void Awake()
    {
        Position = Vector2Int.RoundToInt(transform.position);
    }

    public void GiveResource(ResourceBehaviour resource)
    {
        OnResourceGot?.Invoke(resource);
    }
}
