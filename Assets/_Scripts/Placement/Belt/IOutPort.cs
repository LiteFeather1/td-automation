
public interface IOutPort : IPlaceable
{
	public IInPort[] Ports { get; }
	public Direction OutDirection { get; set; }
}
