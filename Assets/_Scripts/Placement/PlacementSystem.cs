using UnityEngine;
using UnityEngine.Tilemaps;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private Tilemap _groundTilemap;
    [SerializeField] private Tilemap _pathTilemap;

    [Header("Tile Hightlight")]
    [SerializeField] private SpriteRenderer _tileHighlight;
    [SerializeField] private Color _colourNotPlaceble = Color.red;
    private bool _canPlaceBuilding;

    public IPlaceable _placeable;

    public void Update()
    {
        var ray = _camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit))
        {
            var x = Mathf.RoundToInt(hit.point.x);
            var y = Mathf.RoundToInt(hit.point.y);
            var position = new Vector3Int(x, y, 0);

            _canPlaceBuilding = _pathTilemap.HasTile(position) || !_groundTilemap.HasTile(position);
            _tileHighlight.color = _canPlaceBuilding? _colourNotPlaceble : Color.white;

            _tileHighlight.transform.localPosition = position;
        }
    }

    public void SetPlaceable(IPlaceable placeable)
    {
        _placeable = placeable;
    }
}
