using System.Collections.Generic;
using UnityEngine;

public class ResourceCollector : Building, IOutPort
{
    [Header("Resource Collector")]
    [SerializeField] private ResourceType _type;
    [SerializeField] private Transform _range;
    [SerializeField] private float _timeToCollect = 5f;
    [SerializeField] private float _speedPerNode = 2f;

    private float _elapsedTime = 0f;

    private readonly List<ResourceNode> r_resourceNodes = new();

    public IInPort Port { get; set; }
    public Direction OutDirection { get; set; } = Direction.Right;

    private float Range => _range.localScale.x * .5f;

    public override bool CanBeRotated => true;
    public override bool CanBeDestroyed => true;

    internal void Update()
    {
        if (Port == null || !Port.CanReceiveResource(_type))
            return;

        _elapsedTime += Time.deltaTime * _speedPerNode * r_resourceNodes.Count;
        if (_elapsedTime < _timeToCollect)
            return;

        _elapsedTime %= _timeToCollect;

        var resource = r_resourceNodes[Random.Range(0, r_resourceNodes.Count)].CollectResource();
        resource.transform.position = (Vector2)Port.Position;
        Port.ReceiveResource(resource);
        resource.gameObject.SetActive(true);
    }

    internal void OnDisable()
    {
        foreach (var node in r_resourceNodes)
        {
            node.OnDepleted -= OnNodeDepleted;
        }
    }

    public override void Place()
    {
        _range.gameObject.SetActive(false);
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
        if (node.Type != _type || Vector2Int.Distance(Position, node.Position) > Range)
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
}