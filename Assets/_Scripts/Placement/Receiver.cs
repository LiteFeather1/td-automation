using System;
using UnityEngine;

public class Receiver : Building, IInPort
{
    public Direction InDirection { get; set; } = Direction.Any;

    public Action<ResourceBehaviour> OnResourceGot { get; set; }

    public bool CanReceiveResource => true;

    public override bool CanBeRotated => true;
    public override bool CanBeDestroyed => false;

    public void Awake()
    {
        Position = Vector2Int.RoundToInt(transform.position);
    }

    public void GiveResource(ResourceBehaviour resource)
    {
        OnResourceGot?.Invoke(resource);
    }
}
