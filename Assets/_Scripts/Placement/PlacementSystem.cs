using System;
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
    private Sprite _defaultTileHighlight;

    private Vector3Int _mousePos;

    private bool _canPlaceBuilding;
    private Building _building;
    private Direction _buildingOutDirection;

    public Action<Building> OnBuildingPlaced { get; set; }

    public void Awake()
    {
        _defaultTileHighlight = _tileHighlight.sprite;
    }

    public void Update()
    {
        var ray = _camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit))
        {
            var x = Mathf.RoundToInt(hit.point.x);
            var y = Mathf.RoundToInt(hit.point.y);
            _mousePos = new Vector3Int(x, y, 0);

            _canPlaceBuilding = !_pathTilemap.HasTile(_mousePos) || _groundTilemap.HasTile(_mousePos);
            _tileHighlight.color = _canPlaceBuilding ? Color.white : _colourNotPlaceble;

            _tileHighlight.transform.localPosition = _mousePos;
        }
    }

    public void SetPlaceable(PlaceableData placeableData)
    {
        _tileHighlight.sprite = placeableData.Icon;

        _building = placeableData.BuildingPrefab;
        _buildingOutDirection = _building.OutDirection;
    }

    public void PlaceBuilding()
    {
        if (!_canPlaceBuilding || _building == null)
            return;

        var newBuilding = Instantiate(
            _building, _mousePos, _tileHighlight.transform.rotation
        );
        newBuilding.SetRotation(_buildingOutDirection);
        newBuilding.Position = (Vector2Int)_mousePos;

        OnBuildingPlaced?.Invoke(newBuilding);
    }

    public void CancelBuilding()
    {
        _building = null;
        _tileHighlight.sprite = _defaultTileHighlight;
        _tileHighlight.transform.eulerAngles = Vector3.zero;
    }

    public void RotateBuilding()
    {
        _tileHighlight.transform.eulerAngles = new(
            0f, 0f, _tileHighlight.transform.eulerAngles.z - 90f
        );

        _buildingOutDirection = _buildingOutDirection switch
        {
            Direction.Left => Direction.Up,
            Direction.Right => Direction.Down,
            Direction.Up => Direction.Right,
            Direction.Down => Direction.Left,
            _ => _buildingOutDirection,
        };
    }
}
