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
        var inputs = InputManager.Instance.InputSystem.Player;
        inputs.ScrollWheel.performed += ZoomPerformed;
        inputs.DefaultZoom.performed += DefaultZoom;
    }

    internal void Update()
    {
        var inputs = InputManager.Instance.InputSystem.Player;
        if (inputs.Drag.IsPressed())
        {
            MoveCamera(_mouseSpeed * Time.deltaTime * -inputs.Look.ReadValue<Vector2>());
        }
        else if (inputs.Move.IsPressed())
        {
            MoveCamera(_keyboardSpeed * Time.deltaTime * inputs.Move.ReadValue<Vector2>());
        }
    }

    internal void OnDisable()
    {
        var inputs = InputManager.Instance.InputSystem.Player;
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
        var camSizeY = _camera.orthographicSize;
        var camSizeX = camSizeY * 16f / 9f;
        var pos = transform.position;
        pos.x = Mathf.Clamp(pos.x + delta.x, _min.x + camSizeX, _max.x - camSizeX);
        pos.y = Mathf.Clamp(pos.y + delta.y, _min.y + camSizeY, _max.y - camSizeY);
        transform.position = pos;
    }

#if UNITY_EDITOR
    internal void OnDrawGizmos()
    {
        Gizmos.color = sr_boundsColour;

        var topLeft = new Vector3(_min.x, _max.y);
        var bottomRight = new Vector3(_max.x, _min.y);
        Gizmos.DrawLine(topLeft, _max);
        Gizmos.DrawLine(_max, bottomRight);
        Gizmos.DrawLine(bottomRight, _min);
        Gizmos.DrawLine(_min, topLeft);
    }
#endif
}
