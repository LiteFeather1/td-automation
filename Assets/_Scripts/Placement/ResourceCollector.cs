using System.Collections.Generic;
using UnityEngine;

public class ResourceCollector : Building, IOutPort
{
    [Header("Resource Collector")]
    [SerializeField] private ResourceType _type;
    [SerializeField] private float _range = 5f;
    [SerializeField] private float _timeToCollect = 5f;
    [SerializeField] private float _speedPerNode = 2f;

    [Header("References")]
    [SerializeField] private SpriteRenderer _sr;

    private float _elapsedTime = 0f;

    private readonly List<ResourceNode> r_resourceNodes = new();

    public IInPort Port { get; set; }
    public Direction OutDirection { get; set; } = Direction.Right;

    public override bool CanBeRotated => true;
    public override bool CanBeDestroyed => true;

    public void Update()
    {
        if (Port == null || !Port.CanReceiveResource)
            return;

        _elapsedTime += Time.deltaTime * _speedPerNode * r_resourceNodes.Count;
        if (_elapsedTime > _timeToCollect)
        {
            _elapsedTime %= _timeToCollect;
            Port.GiveResource(Instantiate(
                r_resourceNodes[Random.Range(0, r_resourceNodes.Count)].CollectResource(),
                new(Port.Position.x, Port.Position.y),
                Quaternion.identity
            ));
        }
    }

    public void OnDisable()
    {
        foreach (var node in r_resourceNodes)
        {
            node.OnDepleted -= OnNodeDepleted;
        }
    }

    public void TryAddNode(ResourceNode node)
    {
        if (node.Type != _type || Vector2Int.Distance(Position, node.Position) > _range)
            return;

        r_resourceNodes.Add(node);
        node.OnDepleted += OnNodeDepleted;
    }

    public void TryEnable()
    {
        if (r_resourceNodes.Count > 0)
        {
            enabled = true;
            ChangeAlpha(1f);
        }
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

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _range);
    }
}