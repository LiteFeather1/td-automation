using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using LTF.SerializedDictionary;

public class PlacementSystem : MonoBehaviour
{
    private static readonly Dictionary<Direction, float> sr_outDirectionToAngle = new()
    {
        { Direction.Right, 0f },
        { Direction.Down, 270f },
        { Direction.Left, 180f },
        { Direction.Up, 90f },
    };

    [SerializeField] private Camera _camera;
    [SerializeField] private Tilemap _pathTilemap;
    [SerializeField] private SerializedDictionary<Vector2Int, ResourceNode> _resourceNodes = new();

    [Header("Belt")]
    [SerializeField] private BeltPathSystem _beltPathSystem;
    [SerializeField] private Sprite _straightBelt;
    [SerializeField] private Sprite _curvedBelt;

    [Header("Tile Hightlight")]
    [SerializeField] private Transform _tileHighlight;
    [SerializeField] private SpriteRenderer _tileHighlightRenderer;
    [SerializeField] private SpriteRenderer _highlightNotPlaceable;
    [SerializeField] private Color _canPlaceHighlight = Color.blue;
    [SerializeField] private Color _notPlaceableHighlight = Color.red;
    [SerializeField] private Color _nodeHighlight = Color.yellow;

    private Vector2Int _mousePos;

    private bool _canPlaceBuilding;
    private Building _buildingPrefab;
    private Building _buildingToPlace;
    private Direction _inDirection;
    private Direction _outDirection;
    private readonly Dictionary<Vector2Int, Building> r_buildings = new();

    private readonly Dictionary<Vector2Int, IHoverable> r_hoverables = new();
    private IHoverable _currentHoverable;

    public Action<Building> OnBuildingPlaced { get; set; }
    public Action<Building> OnBuildingRemoved { get; set; }

    public Action<ResourceType> OnResourceCollected { get; set; }

    public Action<IHoverable> OnHoverableHovered { get; set; }
    public Action OnHoverableUnhovered { get; set; }

    public BeltPathSystem BeltPathSystem => _beltPathSystem;

    internal void Start()
    {
        foreach (ResourceNode node in _resourceNodes.Values)
        {
            node.OnDepleted += ResourceDepleted;
            r_hoverables.Add(node.Position, node);
        }
    }

    internal void FixedUpdate()
    {
        Vector3Int worldPos = Vector3Int.RoundToInt(_camera.ScreenToWorldPoint(Input.mousePosition));
        worldPos.z = 0;
        Vector2Int mousePos = (Vector2Int)worldPos;
        if (mousePos == _mousePos)
            return;

        _mousePos = mousePos;
        SetCanPlaceBuilding(worldPos);

        _tileHighlight.localPosition = worldPos;

        if (_buildingToPlace != null)
        {
            _tileHighlightRenderer.color = _canPlaceBuilding ? _canPlaceHighlight : _notPlaceableHighlight;
            _highlightNotPlaceable.enabled = !_canPlaceBuilding;

            _buildingToPlace.transform.localPosition = worldPos;

            if (_buildingToPlace is not BeltPath belt || _buildingToPlace.SR.sprite == _straightBelt)
                return;

            _buildingToPlace.SR.sprite = _straightBelt;
            _buildingToPlace.SR.flipY = false;
            belt.SetArrowRotation(0f);
            _outDirection = _beltPathSystem.OppositeDirection(_inDirection);

            return;
        }
        else
        {
            SetHighlightColour();
        }

        TryGetHoverable(mousePos);
    }

    internal void OnDisable()
    {
        foreach (ResourceNode node in _resourceNodes.Values)
        {
            node.OnDepleted -= ResourceDepleted;
        }
    }

    public void SetPlaceable(Building prefab)
    {
        _buildingPrefab = prefab;
        _tileHighlightRenderer.transform.localScale = new(1.5f, 1.5f, 1f);

        if (_buildingToPlace != null)
            Destroy(_buildingToPlace.gameObject);

        InstantiateBuilding((Vector2)_mousePos, Quaternion.identity);

        _inDirection = _buildingToPlace is IInPort inPort ? inPort.InDirection : Direction.None;
        _outDirection = _buildingToPlace is IOutPort outPort ? outPort.OutDirection : Direction.None;
    }

    public void AddBuilding(Building building)
    {
        building.Place();
        r_buildings.Add(building.Position, building);
        r_hoverables.Add(building.Position, building);
        OnBuildingPlaced?.Invoke(building);
    }

    public void LeftClick()
    {
        if (UIMouseBlocker.MouseBlocked)
            return;

        if (_buildingToPlace == null)
        {
            if (_resourceNodes.TryGetValue(_mousePos, out ResourceNode node))
            {
                OnResourceCollected?.Invoke(node.GetResource());

                if (!node.Depleted)
                    OnHoverableHovered?.Invoke(_currentHoverable);
                else
                    UnhoverHoverable();
            }

            return;
        }

        if (!_canPlaceBuilding)
            return;

        _buildingToPlace.Position = _mousePos;

        if (_buildingToPlace is ResourceCollector collector)
        {
            collector.OutDirection = _outDirection;
            foreach (ResourceNode  node in _resourceNodes.Values)
            {
                collector.TryAddNode(node);
            }

            _beltPathSystem.AddOutPort(collector);
            collector.TryEnable();
        }
        else
        {
            if (_buildingToPlace is IInPort inPort)
            {
                inPort.InDirection = _inDirection;
                _beltPathSystem.AddInPort(inPort);
            }

            if (_buildingToPlace is IOutPort outPort)
            {
                outPort.OutDirection = _outDirection;
                _beltPathSystem.AddOutPort(outPort);
            }
        }

        _tileHighlightRenderer.color = _notPlaceableHighlight;
        _highlightNotPlaceable.enabled = true;
        _canPlaceBuilding = false;

        _buildingToPlace.enabled = true;

        Vector3 position = _buildingToPlace.transform.position;
        Quaternion rotation;
        if (_buildingToPlace is BeltPath)
        {
            _inDirection = _beltPathSystem.OppositeDirection(_outDirection);
            rotation = Quaternion.Euler(0f, 0f, sr_outDirectionToAngle[_outDirection]);
        }
        else
        {
            rotation = _buildingToPlace.transform.rotation;
        }

        AddBuilding(_buildingToPlace);

        if (_buildingPrefab != null)
            InstantiateBuilding(position, rotation);
    }

    public void UnselectBuildingBuilding()
    {
        if (_buildingToPlace == null)
            return;

        _tileHighlight.position = _buildingToPlace.transform.position;
        _tileHighlightRenderer.transform.localScale = Vector3.one;
        _highlightNotPlaceable.enabled = false;

        SetCanPlaceBuilding((Vector3Int)_mousePos);
        SetHighlightColour();

        TryGetHoverable(_mousePos);

        _buildingToPlace = null;
        _buildingPrefab = null;
    }

    public void TryCancelOrDesconstructBuilding()
    {
        if (_buildingToPlace != null)
        {
            Destroy(_buildingToPlace.gameObject);
            UnselectBuildingBuilding();
        }
        else if (r_buildings.TryGetValue(_mousePos, out Building building) && building.CanBeDestroyed)
        {
            r_buildings.Remove(_mousePos);
            r_hoverables.Remove(_mousePos);

            building.Destroy();
            UnhoverHoverable();

            OnBuildingRemoved?.Invoke(building);
            _beltPathSystem.TryRemovePosition(building.Position);
            OnHoverableUnhovered?.Invoke();
        }
    }

    public void RotateBuilding()
    {
        if (_buildingToPlace == null || !_buildingToPlace.CanBeRotated)
            return;

        if (_buildingToPlace is BeltPath beltPath && _beltPathSystem.HasOutPortAt(_mousePos, _inDirection))
        {
            do
                _outDirection = RotateDirection(_outDirection);
            while (_outDirection == _inDirection);

            SpriteRenderer sr = _buildingToPlace.SR;
            float zRot;
            if (sr.sprite == _straightBelt)
            {
                sr.sprite = _curvedBelt;
                zRot = 270f;
            }
            else if (sr.flipY)
            {
                sr.sprite = _straightBelt;
                sr.flipY = false;
                zRot = 0f;
            }
            else
            {
                sr.flipY = true;
                zRot = 90f;
            }

            beltPath.SetArrowRotation(zRot);
        }
        else
        {
            _buildingToPlace.transform.eulerAngles = new(
                0f, 0f, _buildingToPlace.transform.eulerAngles.z - 90f
            );
            _inDirection = RotateDirection(_inDirection);
            _outDirection = RotateDirection(_outDirection);
        }

        static Direction RotateDirection(Direction dir) => dir switch
        {
            Direction.Left => Direction.Up,
            Direction.Right => Direction.Down,
            Direction.Up => Direction.Right,
            Direction.Down => Direction.Left,
            _ => dir,
        };
    }

    public bool TryGetBuilding(out Building building)
    {
        return r_buildings.TryGetValue(_mousePos, out building);
    }

    public void UnhoverHoverable()
    {
        OnHoverableUnhovered?.Invoke();
        _currentHoverable?.Unhover();
        _currentHoverable = null;
    }

    private void ResourceDepleted(ResourceNode node)
    {
        node.OnDepleted -= ResourceDepleted;
        _resourceNodes.Remove(node.Position);
        r_hoverables.Remove(node.Position);
    }

    private void SetCanPlaceBuilding(Vector3Int worldPos)
    {
        _canPlaceBuilding = !r_buildings.ContainsKey(_mousePos)
            && !_pathTilemap.HasTile(worldPos)
            && !_resourceNodes.ContainsKey(new(worldPos.x, worldPos.y));
    }

    private void SetHighlightColour()
    {
        if (_resourceNodes.ContainsKey(_mousePos))
            _tileHighlightRenderer.color = _nodeHighlight;
        else
            _tileHighlightRenderer.color = _canPlaceBuilding ? Color.white : _notPlaceableHighlight;
    }

    private void TryGetHoverable(Vector2Int pos)
    {
        if (!UIMouseBlocker.MouseBlocked && r_hoverables.TryGetValue(pos, out IHoverable hoverable))
        {
            OnHoverableHovered?.Invoke(hoverable);
            _currentHoverable?.Unhover();
            hoverable.Hover();
            _currentHoverable = hoverable;
        }
        else if (_currentHoverable != null)
        {
            UnhoverHoverable();
        }
    }

    private void InstantiateBuilding(Vector3 position, Quaternion rotation)
    {
        _buildingToPlace = Instantiate(
            _buildingPrefab, position, rotation, transform
        );

        _buildingToPlace.enabled = false;
    }

    public void AddHoverable(Vector2Int position, IHoverable hoverable)
    {
        r_hoverables.Add(position, hoverable);
    }

#if UNITY_EDITOR
    [ContextMenu("Get Resource Nodes")]
    internal void GetResourceNodes()
    {
        _resourceNodes = new();
        foreach (ResourceNode node in GetComponentsInChildren<ResourceNode>())
        {
            node.Position = Vector2Int.RoundToInt(node.transform.position);
            _resourceNodes.Add(node.Position, node);
            UnityEditor.EditorUtility.SetDirty(node);
        }
        UnityEditor.EditorUtility.SetDirty(this);
    }
#endif
}
