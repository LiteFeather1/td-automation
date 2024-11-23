
public interface IInPort : IPlaceable
{
	public Direction InDirection { get; set; }

	public bool CanReceiveResource(ResourceType type);

	public void ReceiveResource(ResourceBehaviour resource);
}