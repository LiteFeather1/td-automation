using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : Singleton<CameraManager>
{
    [Header("Z")]
    [SerializeField] private float _defaultZ = -15f;
    [SerializeField] private Vector2 _zRange = new(-10f, -20f);
    [SerializeField] private float _zSpeed;

    [Header("Borders")]
    [SerializeField] private Vector2 _min;
    [SerializeField] private Vector2 _max;

    private void OnEnable()
    {
        var inputs = InputManager.Instance.InputSystem.Player;
        inputs.ScrollWheel.performed += ZoomPerformed;
    }

    private void OnDisable()
    {
        var inputs = InputManager.Instance.InputSystem.Player;
        inputs.ScrollWheel.performed -= ZoomPerformed;
    }

    private void ZoomPerformed(InputAction.CallbackContext ctx)
    {
        var pos = transform.position;
        pos.z = Mathf.Clamp(
            pos.z + ctx.ReadValue<Vector2>().y * _zSpeed, _zRange.x, _zRange.y
        );
        transform.position = pos;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        var topLeft = new Vector3(_min.x, _max.y);
        var bottomRight = new Vector3(_max.x, _min.y);
        Gizmos.DrawLine(topLeft, _max);
        Gizmos.DrawLine(_max, bottomRight);
        Gizmos.DrawLine(bottomRight, _min);
        Gizmos.DrawLine(_min, topLeft);
    }
#endif
}
