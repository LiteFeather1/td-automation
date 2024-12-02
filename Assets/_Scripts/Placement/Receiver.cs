using System;
using UnityEngine;

public class Receiver : InPort
{
    public override Direction InDirection { get; set; } = Direction.Any;

    public Action<ResourceBehaviour> OnResourceGot { get; set; }

    public override bool CanBeDestroyed => false;

    public override void ReceiveResource(ResourceBehaviour resource)
    {
        OnResourceGot?.Invoke(resource);
        _resource = null;
    }
}
