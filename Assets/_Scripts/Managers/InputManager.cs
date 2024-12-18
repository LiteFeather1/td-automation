public class InputManager : Singleton<InputManager>
{
    public InputSystem_Actions InputSystem { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        InputSystem = new();
        InputSystem.Player.Enable();
    }

    public void Disable()
    {
        Instance = null;
        InputSystem.Disable();
    }
}
