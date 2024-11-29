
public interface IOutPort : IPlaceable
{
	public IInPort GetPort(int index);
	public void SetPort(IInPort inPort);
	public Direction OutDirection { get; set; }
}
