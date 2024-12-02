using UnityEngine;

public interface IInPort : IPlaceable
{
	public Direction InDirection { get; set; }

	public Vector2 WorldPosition { get; }

	public ResourceBehaviour Resource { get; }

	public bool CanReceiveResource(ResourceType type);

	public void ReceiveResource(ResourceBehaviour resource);
}