using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private Transform _tileHighlight;

    private void Update()
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit))
        {
            var x = Mathf.Round(hit.point.x);
            var y = Mathf.Round(hit.point.y);
            _tileHighlight.position = new Vector3(x, y, 0f);
        }
    }
}
