
public interface IOutPort : IPlaceable
{
	public void SetPort(IInPort inPort);
	public Direction OutDirection { get; set; }
}
