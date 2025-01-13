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
    private IInPort _inPort;
    private IOutPort _outPort;
    private readonly Dictionary<Vector2Int, Building> r_buildings = new();

    private readonly Dictionary<Vector2Int, IHoverable> r_hoverables = new();
    private IHoverable _currentHoverable;

    public Action<Building> OnBuildingPlaced { get; set; }
    public Action<Building> OnBuildingRemoved { get; set; }

    public Action<ResourceType> OnResourceCollected { get; set; }

    public Action<IHoverable> OnHoverableHovered { get; set; }
    public Action OnHoverableUnhovered { get; set; }

    public BeltPathSystem BeltPathSystem => _beltPathSystem;

    private void Start()
    {
        foreach (ResourceNode node in _resourceNodes.Values)
        {
            node.OnDepleted += ResourceDepleted;
            r_hoverables.Add(node.Position, node);
        }
    }

    private void Update()
    {
        if (UIMouseBlocker.MouseBlocked)
            return;

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
            _outPort.OutDirection = _beltPathSystem.OppositeDirection(_inPort.InDirection);

            return;
        }
        else
        {
            SetHighlightColour();
        }

        TryGetHoverable(mousePos);
    }

    private void OnDisable()
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
            else
                TryCollectResource();

            return;
        }

        if (!_canPlaceBuilding)
            return;

        _buildingToPlace.Position = _mousePos;

        if (_buildingToPlace is ResourceCollector collector)
        {
            foreach (ResourceNode  node in _resourceNodes.Values)
            {
                collector.TryAddNode(node);
            }

            _beltPathSystem.AddOutPort(collector);
            collector.TryEnable();
        }
        else
        {
            if (_inPort != null)
            {
                _beltPathSystem.AddInPort(_inPort);
            }

            if (_outPort != null)
            {
                _beltPathSystem.AddOutPort(_outPort);
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
            _inPort.InDirection = _beltPathSystem.OppositeDirection(_outPort.OutDirection);
            rotation = Quaternion.Euler(0f, 0f, sr_outDirectionToAngle[_outPort.OutDirection]);
        }
        else
        {
            rotation = _buildingToPlace.transform.rotation;
        }

        AddBuilding(_buildingToPlace);

        if (_buildingPrefab != null)
        {
            IInPort prevInPort = _inPort;
            IOutPort prevOutPort = _outPort;
            InstantiateBuilding(position, rotation);

            if (_inPort != null)
                _inPort.InDirection = prevInPort.InDirection;

            if (_outPort != null)
                _outPort.OutDirection = prevOutPort.OutDirection;
        }
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
            TryCollectResource();

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

        if (_buildingToPlace is BeltPath beltPath && _beltPathSystem.HasOutPortAt(_mousePos, beltPath.InDirection))
        {
            do
                _outPort.OutDirection = RotateDirection(_outPort.OutDirection);
            while (_outPort.OutDirection == beltPath.InDirection);

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

            if (_inPort != null)
                _inPort.InDirection = RotateDirection(_inPort.InDirection);

            if (_outPort != null)
                _outPort.OutDirection = RotateDirection(_outPort.OutDirection);
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

    public void AddHoverable(Vector2Int position, IHoverable hoverable)
    {
        r_hoverables.Add(position, hoverable);
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

        _inPort = _buildingToPlace as IInPort;
        _outPort = _buildingToPlace as IOutPort;

        _buildingToPlace.enabled = false;
    }

    private void TryCollectResource()
    {
        if (_beltPathSystem.InPorts.TryGetValue(_mousePos, out IInPort inPort))
        {
            if (inPort.Resource != null)
                OnResourceCollected?.Invoke(inPort.CollectResource());
        }
        else if (
            _beltPathSystem.OutPorts.TryGetValue(_mousePos, out IOutPort outPort)
            && outPort is ResourceCollector resourceCollector
        )
        {
            if (resourceCollector.HasResource)
                OnResourceCollected?.Invoke(resourceCollector.CollectResource());
        }
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
