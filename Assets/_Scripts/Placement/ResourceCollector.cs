using System.Collections.Generic;
using UnityEngine;

public class ResourceCollector : Building, IOutPort
{
    private static readonly int sr_SizeId = Shader.PropertyToID("_Size");

    [Header("Resource Collector")]
    [SerializeField] private ResourceType _type;
    [SerializeField] private Transform _range;
    [SerializeField] private float _timeToCollect = 5f;
    [SerializeField] private float _speedMultiplierPerNode = 2f;
    [SerializeField] private SpriteRenderer _indicator;

    private float _elapsedTime = 0f;

    private ResourceBehaviour _resource;

    private IInPort _port;

    private readonly List<ResourceNode> r_resourceNodes = new();

    public Direction OutDirection { get; set; } = Direction.Right;

    public override bool CanBeRotated => true;
    public override bool CanBeDestroyed => true;

    private float Range => _range.localScale.x * .5f;

    internal void Update()
    {
        if (_resource == null)
        {
            _elapsedTime += Time.deltaTime * _speedMultiplierPerNode * r_resourceNodes.Count;

            _indicator.material.SetFloat(
                sr_SizeId,
                Mathf.Lerp(0f, _indicator.transform.localScale.x, _elapsedTime / _timeToCollect)
            );

            if (_elapsedTime < _timeToCollect)
                return;

            _elapsedTime %= _timeToCollect;

            _resource = r_resourceNodes[Random.Range(0, r_resourceNodes.Count)].CollectResource();
            _resource.transform.position = transform.position;
            _indicator.material.SetFloat(sr_SizeId, 0f);
            _resource.gameObject.SetActive(true);
        }
        else if (_port != null && _port.CanReceiveResource(_type))
        {
            _port.ReceiveResource(_resource);
            _resource = null;
        }
    }

    internal void OnDisable()
    {
        foreach (var node in r_resourceNodes)
        {
            node.OnDepleted -= OnNodeDepleted;
        }

        if (_port != null)
            _port.OnDestroyed -= PortDestroyed;
    }

    public void SetPort(IInPort inPort)
    {
        _port = inPort;
        inPort.OnDestroyed += PortDestroyed;
    }

    public override void Place()
    {
        _range.gameObject.SetActive(false);
    }

    public override void Destroy()
    {
        base.Destroy();
        _resource?.Deactive();
    }

    public override void Hover()
    {
        _range.gameObject.SetActive(true);
    }

    public override void Unhover()
    {
        _range.gameObject.SetActive(false);
    }

    public void TryAddNode(ResourceNode node)
    {
        var dir = Position - node.Position;
        var mag = (int)dir.magnitude;
        dir = new(dir.x / mag, dir.y / mag);
        if (node.Type != _type
            || Vector2Int.Distance(Position - dir, node.Position) > Range
        )
            return;

        r_resourceNodes.Add(node);
        node.OnDepleted += OnNodeDepleted;
    }

    public void TryEnable()
    {
        ChangeAlpha((enabled = r_resourceNodes.Count > 0) ? 1f : .5f);
    }

    private void OnNodeDepleted(ResourceNode node)
    {
        r_resourceNodes.Remove(node);
        node.OnDepleted -= OnNodeDepleted;

        if (r_resourceNodes.Count == 0)
        {
            ChangeAlpha(.5f);
        }
    }

    private void ChangeAlpha(float alpha)
    {
        var colour = _sr.color;
        colour.a = alpha;
        _sr.color = colour;
    }

    private void PortDestroyed(Vector2Int _)
    {
        _port.OnDestroyed -= PortDestroyed;
        _port = null;
    }
}