using System;
using UnityEngine;

public class Receiver : Building, IInPort
{
    public Direction InDirection { get; set; } = Direction.Any;

    public Action<ResourceBehaviour> OnResourceGot { get; set; }


    public override bool CanBeRotated => true;
    public override bool CanBeDestroyed => false;

    public bool CanReceiveResource(ResourceType _) => true;

    public void ReceiveResource(ResourceBehaviour resource)
    {
        OnResourceGot?.Invoke(resource);
    }
}
