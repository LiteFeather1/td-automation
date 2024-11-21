
public interface IInPut : IPlaceable
{

	public Direction InDirection { get; set; }

	public bool CanReceiveResource { get; }

	public void GiveResource(ResourceBehaviour resource);
}