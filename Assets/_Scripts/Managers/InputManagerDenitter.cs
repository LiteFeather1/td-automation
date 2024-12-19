using UnityEngine;

public class InputManagerDenitter : MonoBehaviour
{
    [SerializeField] private InputManager _inputManager;

    private void OnDisable()
    {
        _inputManager.Disable();
    }
}
