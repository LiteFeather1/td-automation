public class InputManager : PersistentSingleton<InputManager>
{
    public InputSystem_Actions InputSystem { get; private set; }

    protected override void InitialiseInstance()
    {
        base.InitialiseInstance();
        InputSystem = new();
        InputSystem.Enable();
    }

    public void EnableInputSystem() => InputSystem.Enable();

    public void DisableInputSystem() => InputSystem.Disable();
}
