using System;

public class Receiver : InPort
{
    public Action<ResourceBehaviour> OnResourceGot { get; set; }

    public override bool CanBeDestroyed => false;

    internal void Awake() => InDirection = Direction.Any;

    public override void ResourceCentralized()
    {
        OnResourceGot?.Invoke(_resource);
        _resource = null;
    }
}
