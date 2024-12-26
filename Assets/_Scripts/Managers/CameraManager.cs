using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    private static readonly Color sr_boundsColour = new(0.94f, 0.39f, 0.01f, 1f);

    [SerializeField] private Camera _camera;

    [Header("Z")]
    [SerializeField] private float _defaultZ = 10f;
    [SerializeField] private Vector2 _zRange = new(5f, 15f);
    [SerializeField] private float _zSpeed;

    [Header("Move")]
    [SerializeField] private float _keyboardSpeed = 5f;
    [SerializeField] private float _mouseSpeed = 2f;
    [SerializeField] private Vector2 _min;
    [SerializeField] private Vector2 _max;

    internal void OnEnable()
    {
        InputSystem_Actions.PlayerActions inputs = InputManager.Instance.InputSystem.Player;
        inputs.ScrollWheel.performed += ZoomPerformed;
        inputs.DefaultZoom.performed += DefaultZoom;
    }

    internal void Update()
    {
        InputSystem_Actions.PlayerActions inputs = InputManager.Instance.InputSystem.Player;

        if (inputs.Drag.IsPressed())
        {
            MoveCameraInput(_mouseSpeed * -inputs.Look.ReadValue<Vector2>());
        }
        else if (inputs.Move.IsPressed())
        {
            MoveCameraInput(_keyboardSpeed * inputs.Move.ReadValue<Vector2>());
        }

        void MoveCameraInput(Vector2 delta)
        {
            MoveCamera(delta * (
                Helpers.Map(_camera.orthographicSize, _zRange.x, _zRange.y, .25f, 1f)
                * Time.deltaTime
                / Time.timeScale
            ));
        }
    }

    internal void OnDisable()
    {
        InputSystem_Actions.PlayerActions inputs = InputManager.Instance.InputSystem.Player;
        inputs.ScrollWheel.performed -= ZoomPerformed;
        inputs.DefaultZoom.performed -= DefaultZoom;
    }

    private void ZoomPerformed(InputAction.CallbackContext ctx)
    {
        _camera.orthographicSize = Mathf.Clamp(
            _camera.orthographicSize - ctx.ReadValue<Vector2>().y * _zSpeed, _zRange.x, _zRange.y
        );

        MoveCamera(Vector2.zero);
    }

    private void DefaultZoom(InputAction.CallbackContext _)
    {
        _camera.orthographicSize = _defaultZ;
    }

    private void MoveCamera(Vector2 delta)
    {
        float camSizeY = _camera.orthographicSize;
        float camSizeX = camSizeY * 16f / 9f;
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x + delta.x, _min.x + camSizeX, _max.x - camSizeX);
        pos.y = Mathf.Clamp(pos.y + delta.y, _min.y + camSizeY, _max.y - camSizeY);
        transform.position = pos;
    }

#if UNITY_EDITOR
    internal void OnDrawGizmos()
    {
        Gizmos.color = sr_boundsColour;

        Vector3 topLeft = new(_min.x, _max.y);
        Vector3 bottomRight = new(_max.x, _min.y);
        Gizmos.DrawLine(topLeft, _max);
        Gizmos.DrawLine(_max, bottomRight);
        Gizmos.DrawLine(bottomRight, _min);
        Gizmos.DrawLine(_min, topLeft);
    }
#endif
}
