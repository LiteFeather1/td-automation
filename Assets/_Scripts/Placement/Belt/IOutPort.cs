
public interface IOutPort : IPlaceable
{
	public IInPort Port { get; set; }
	public Direction OutDirection { get; set; }
}
