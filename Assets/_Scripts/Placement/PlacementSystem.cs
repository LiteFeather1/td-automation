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

    private Vector2Int _mousePos;

    private bool _canPlaceBuilding;
    private Building _buildingPrefab;
    private Building _buildingToPlace;
    private Direction _inDirection;
    private Direction _outDirection;
    private readonly Dictionary<Vector2Int, Building> r_buildings = new();

    private readonly Dictionary<Vector2Int, IHoverable> r_hoverables = new();
    private bool _isHoveringHoverable;

    public Action<Building> OnBuildingPlaced { get; set; }
    public Action<Building> OnBuildingRemoved { get; set; }

    public Action<ResourceType> OnResourceCollected { get; set; }

    public Action<IHoverable> OnHoverableHovered { get; set; }
    public Action OnHoverableUnhovered { get; set; }

    internal void Start()
    {
        foreach (var node in _resourceNodes.Values)
        {
            node.OnDepleted += ResourceDepleted;
            r_hoverables.Add(node.Position, node);
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
        if (_buildingToPlace != null)
        {
            _buildingToPlace.transform.localPosition = mousePos;
        }
        else if (r_hoverables.TryGetValue(_mousePos, out var hoverable))
        {
            OnHoverableHovered?.Invoke(hoverable);
            _isHoveringHoverable = true;
        }
        else if (_isHoveringHoverable)
        {
            OnHoverableUnhovered?.Invoke();
            _isHoveringHoverable = false;
        }
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
        _tileHighlight.enabled = false;
        _tileHighlight.transform.eulerAngles = new(0f, 0f, 0f);

        _buildingPrefab = placeableData.BuildingPrefab;

        _buildingToPlace = Instantiate(
            _buildingPrefab, _tileHighlight.transform.position, Quaternion.identity
        );

        _inDirection = _buildingToPlace is IInPort inPort ? inPort.InDirection : Direction.None;
        _outDirection = _buildingToPlace is IOutPort outPort ? outPort.OutDirection : Direction.None;
    }

    public void AddBuilding(Building building)
    {
        r_buildings.Add(building.Position, building);
        OnBuildingPlaced?.Invoke(building);
    }

    public void LeftClick()
    {
        if (_buildingToPlace == null)
        {
            if (_resourceNodes.TryGetValue(_mousePos, out var node))
                OnResourceCollected?.Invoke(node.GetResource());

            return;
        }

        if (!_canPlaceBuilding)
            return;

        _buildingToPlace.Position = _mousePos;

        if (_buildingToPlace is IInPort inPort)
        {
            inPort.InDirection = _outDirection;
        }

        if (_buildingToPlace is IOutPort outPort)
        {
            outPort.OutDirection = _outDirection;
        }

        if (_buildingToPlace is ResourceCollector collector)
        {
            foreach (var node in _resourceNodes.Values)
            {
                collector.TryAddNode(node);
            }

            collector.TryEnable();
        }

        AddBuilding(_buildingToPlace);
        _buildingToPlace = Instantiate(
            _buildingPrefab, _tileHighlight.transform.position, _buildingToPlace.transform.rotation
        );
    }

    public void TryCancelOrDesconstructBuilding()
    {
        if (_buildingToPlace != null)
        {
            Destroy(_buildingToPlace.gameObject);
            _buildingPrefab = null;

            _tileHighlight.enabled = true;
            _tileHighlight.transform.eulerAngles = Vector3.zero;
        }
        else if (r_buildings.TryGetValue(_mousePos, out var building) && building.CanBeDestroyed)
        {
            r_buildings.Remove(_mousePos);
            building.Destroy();
            OnBuildingRemoved?.Invoke(building);
        }
    }

    public void RotateBuilding()
    {
        if (_buildingToPlace == null && !_buildingToPlace.CanBeRotated)
            return;

        _buildingToPlace.transform.eulerAngles = new(
            0f, 0f, _buildingToPlace.transform.eulerAngles.z - 90f
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
