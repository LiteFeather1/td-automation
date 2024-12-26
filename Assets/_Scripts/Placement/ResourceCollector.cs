using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceCollector : Building, IOutPort
{
    private const float ANIMATION_TIME = .2f;

    private static readonly int sr_SizeId = Shader.PropertyToID("_Size");

    [Header("Resource Collector")]
    [SerializeField] private ResourceType _type;
    [SerializeField] private Transform _range;
    [SerializeField] private float _timeToCollect = 5f;
    [SerializeField] private float _speedMultiplierPerNode = 2f;
    [SerializeField] private SpriteRenderer _indicator;
    [SerializeField] private LineRenderer _line;

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

            ResourceNode node = r_resourceNodes[Random.Range(0, r_resourceNodes.Count)];
            _resource = node.CollectResource();
            _resource.transform.position = transform.position;
            _indicator.material.SetFloat(sr_SizeId, 0f);
            _resource.gameObject.SetActive(true);

            StartCoroutine(Animation(node.transform.position));
        }
        else if (_port != null && _port.CanReceiveResource(_type))
        {
            _port.ReceiveResource(_resource);
            _resource = null;
        }
    }

    internal void OnDisable()
    {
        foreach (ResourceNode node in r_resourceNodes)
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
        Vector2Int dir = Position - node.Position;
        int mag = (int)dir.magnitude;
        dir = new(dir.x / mag, dir.y / mag);
        if (
            node.Type != _type || Vector2Int.Distance(Position - dir, node.Position) > Range
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
        Color colour = _sr.color;
        colour.a = alpha;
        _sr.color = colour;
    }

    private void PortDestroyed(Vector2Int _)
    {
        _port.OnDestroyed -= PortDestroyed;
        _port = null;
    }

    private IEnumerator Animation(Vector3 endPos)
    {
        Color colour = _line.startColor;
        _line.SetPosition(0, transform.position);
        _line.SetPosition(1, endPos);

        float eTime = 0f;
        while (eTime < ANIMATION_TIME)
        {
            Set(1f - Helpers.EaseInOutCubic(eTime / ANIMATION_TIME));
            eTime += Time.deltaTime;
            yield return null;
        }

        Set(0f);

        void Set(float t)
        {
            colour.a = Mathf.Lerp(0f, .5f, t);
            _line.startColor = colour;
            _line.endColor = colour;
            _line.SetPosition(1, Vector3.Lerp(transform.position, endPos, t));
        }
    }
}