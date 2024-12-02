using UnityEngine;

public abstract class InPort : Building, IInPort
{
    protected ResourceBehaviour _resource;

    public virtual Direction InDirection { get; set; } = Direction.Left;

    public override bool CanBeRotated => true;

    public override bool CanBeDestroyed => true;

    public Vector2 WorldPosition => WorldPosition;

    public ResourceBehaviour Resource => _resource;

    public bool CanReceiveResource(ResourceType type)
    {
        return _resource == null;
    }

    public void ReceiveResource(ResourceBehaviour resource)
    {
        _resource = resource;
    }

    public override void Destroy()
    {
        base.Destroy();
        StopAllCoroutines();
    }
}
