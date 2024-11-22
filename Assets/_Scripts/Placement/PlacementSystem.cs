using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using LTF.SerializedDictionary;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private Tilemap _groundTilemap;
    [SerializeField] private Tilemap _pathTilemap;
    [SerializeField] private SerializedDictionary<Vector2Int, ResourceNode> _resourceNodes = new();

    [Header("Tile Hightlight")]
    [SerializeField] private SpriteRenderer _tileHighlight;
    [SerializeField] private Color _colourNotPlaceble = Color.red;
    private Sprite _defaultTileHighlight;

    private Vector2Int _mousePos;

    private bool _canPlaceBuilding;
    private Building _building;
    private Direction _inDirection;
    private Direction _outDirection;
    private readonly Dictionary<Vector2Int, Building> r_buildings = new();

    public Action<Building> OnBuildingPlaced { get; set; }
    public Action<Vector2Int> OnBuildingRemoved { get; set; }

    public Action<ResourceType> OnResourceCollected { get; set; }

    public void Awake()
    {
        _defaultTileHighlight = _tileHighlight.sprite;
    }

    internal void Start()
    {
        foreach (var node in _resourceNodes.Values)
        {
            node.OnDepleted += ResourceDepleted;
        }
    }

    public void FixedUpdate()
    {
        var mousePos = Vector3Int.RoundToInt(_camera.ScreenToWorldPoint(Input.mousePosition));
        mousePos.z = 0;
        _mousePos = (Vector2Int)mousePos;

        _canPlaceBuilding = (
            !r_buildings.ContainsKey(_mousePos)
            && !_resourceNodes.ContainsKey(_mousePos)
            && !_pathTilemap.HasTile(mousePos)
            && _groundTilemap.HasTile(mousePos)
        );
        _tileHighlight.color = _canPlaceBuilding ? Color.white : _colourNotPlaceble;

        _tileHighlight.transform.localPosition = mousePos;
    }

    internal void OnDisable()
    {
        foreach (var node in _resourceNodes.Values)
        {
            node.OnDepleted -= ResourceDepleted;
        }
    }

    public void SetPlaceable(PlaceableData placeableData)
    {
        _tileHighlight.sprite = placeableData.Icon;
        _tileHighlight.transform.eulerAngles = new(0f, 0f, 0f);

        _building = placeableData.BuildingPrefab;

        _inDirection = _building is IInPort inPort ? inPort.InDirection : Direction.None;
        _outDirection = _building is IOutPort outPort ? outPort.OutDirection : Direction.None;
    }

    public void AddBuilding(Building building)
    {
        r_buildings.Add(building.Position, building);
        OnBuildingPlaced?.Invoke(building);
    }

    public void LeftClick()
    {
        if (_building == null)
        {
            if (_resourceNodes.TryGetValue(_mousePos, out var node))
                OnResourceCollected?.Invoke(node.GetResource());

            return;
        }

        if (!_canPlaceBuilding)
            return;

        var newBuilding = Instantiate(
            _building, new(_mousePos.x, _mousePos.y), _tileHighlight.transform.rotation
        );
        newBuilding.Position = _mousePos;

        if (newBuilding is IInPort inPort)
        {
            inPort.InDirection = _outDirection;
        }

        if (newBuilding is IOutPort outPort)
        {
            outPort.OutDirection = _outDirection;
        }

        if (newBuilding is ResourceCollector collector)
        {
            foreach (var node in _resourceNodes.Values)
            {
                collector.TryAddNode(node);
            }
            
            collector.TryEnable();
        }

        AddBuilding(newBuilding);
    }

    public void TryCancelOrDesconstructBuilding()
    {
        if (_building != null)
        {
            _building = null;
            _tileHighlight.sprite = _defaultTileHighlight;
            _tileHighlight.transform.eulerAngles = Vector3.zero;
        }
        else if (r_buildings.TryGetValue(_mousePos, out var building) && building.CanBeDestroyed)
        {
            r_buildings.Remove(_mousePos);
            building.Destroy();
            OnBuildingRemoved?.Invoke(_mousePos);
        }
    }

    public void RotateBuilding()
    {
        if (_building != null && !_building.CanBeRotated)
            return;

        _tileHighlight.transform.eulerAngles = new(
            0f, 0f, _tileHighlight.transform.eulerAngles.z - 90f
        );

        _inDirection = RotateDirection(_inDirection);
        _outDirection = RotateDirection(_outDirection);

        static Direction RotateDirection(Direction dir) => dir switch
        {
            Direction.Left => Direction.Up,
            Direction.Right => Direction.Down,
            Direction.Up => Direction.Right,
            Direction.Down => Direction.Left,
            _ => dir,
        };
    }

    private void ResourceDepleted(ResourceNode node)
    {
        node.OnDepleted -= ResourceDepleted;
        _resourceNodes.Remove(node.Position);
    }

#if UNITY_EDITOR
    [ContextMenu("Get Resource Nodes")]
    internal void GetResourceNodes()
    {
        _resourceNodes = new();
        foreach (var node in GetComponentsInChildren<ResourceNode>())
        {
            node.Position = Vector2Int.RoundToInt(node.transform.position);
            _resourceNodes.Add(node.Position, node);
            UnityEditor.EditorUtility.SetDirty(node);
        }
        UnityEditor.EditorUtility.SetDirty(this);
    }
#endif
}
