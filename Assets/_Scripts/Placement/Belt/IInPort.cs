
public interface IInPort : IPlaceable
{
	public Direction InDirection { get; set; }

	public bool CanReceiveResource { get; }

	public void GiveResource(ResourceBehaviour resource);
}