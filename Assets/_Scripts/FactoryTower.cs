using System;
using System.Collections.Generic;
using UnityEngine;

public class FactoryTower : MonoBehaviour
{
    private static readonly int sr_midColourID = Shader.PropertyToID("_MiddleColour");
    private static readonly int sr_topInfluenceID = Shader.PropertyToID("_TopInfluence");
    private static readonly int sr_botInfluenceID = Shader.PropertyToID("_BotInfluence");

    [SerializeField] private Health _health;
    [SerializeField] private Receiver[] _receivers;
    [SerializeField] private Tower[] _starterTowers;

    [SerializeField] private SpriteRenderer _sr;
    [SerializeField] private Color _topColour;
    [SerializeField] private Color _botColour;

    private readonly Dictionary<ResourceType, int> r_resources = new();

    public delegate void ResourceAdded(ResourceType type, int totalAmount);
    public ResourceAdded OnResourceModified { get; set; }
    public ResourceAdded OnResourceAdded { get; set; }

    public Action<Dictionary<ResourceType, int>> OnResourcesModified { get; set; }
    public Action<Dictionary<ResourceType, int>> OnResourcesRefundAdded { get; set; }

    public Action<float, Color> OnDamaged { get; set; }

    public Health Health => _health;

    public Receiver[] Receivers => _receivers;
    public Tower[] StarterTowers => _starterTowers;

    public IDictionary<ResourceType, int> Resources => r_resources;

    public void Awake()
    {
        foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
            r_resources.Add(type, 0);

        foreach (Tower tower in _starterTowers)
            foreach (KeyValuePair<ResourceType, int> cost in tower.Data.ResourcesCost)
                r_resources[cost.Key] -= cost.Value;

        SetGradient(1f, _topColour);
    }

    public void OnEnable()
    {
        _health.OnDamageTaken += DamageTaken;
        _health.OnDied += Died;

        foreach (Receiver receiver in _receivers)
        {
            receiver.OnResourceGot += OnResourceGot;
        }
    }

    public void OnDisable()
    {
        _health.OnDamageTaken -= DamageTaken;
        _health.OnDied -= Died;

        foreach (Receiver receiver in _receivers)
        {
            receiver.OnResourceGot -= OnResourceGot;
        }
    }

    public int GetResourceAmount(ResourceType type) => r_resources[type];

    public void AddResource(ResourceType type)
    {
        ModifyResource(type, 1);
        OnResourceAdded?.Invoke(type, 1);
    }

    public void ModifyResource(ResourceType type, int amount)
    {
        r_resources[type] += amount;
        OnResourceModified?.Invoke(type, r_resources[type]);
    }

    public void RemoveResources(Dictionary<ResourceType, int> resources)
    {
        foreach (KeyValuePair<ResourceType, int> resource in resources)
        {
            r_resources[resource.Key] += resource.Value;
        }

        OnResourcesModified?.Invoke(r_resources);
    }

    public void BuildingRefund(Dictionary<ResourceType, int> resources)
    {
        foreach (KeyValuePair<ResourceType, int> resource in resources)
        {
            r_resources[resource.Key] -= Mathf.FloorToInt(resource.Value * .5f);
        }

        OnResourcesModified?.Invoke(r_resources);
        OnResourcesRefundAdded?.Invoke(r_resources);
    }

    public bool HasEnoughResourceToBuild(IDictionary<ResourceType, int> resourceCosts)
    {
        foreach (KeyValuePair<ResourceType, int> resourceCost in resourceCosts)
            if (r_resources[resourceCost.Key] < -resourceCost.Value)
                return false;

        return true;
    }

    private void SetGradient(float t, Color colour)
    {
        _sr.material.SetColor(sr_midColourID, colour);

        const float MAX_INFLUENCE = .5f;
        float topInfluence = Helpers.Map(t, 1f, 0f, MAX_INFLUENCE, 0f);
        _sr.material.SetFloat(sr_topInfluenceID, topInfluence);
        _sr.material.SetFloat(sr_botInfluenceID, MAX_INFLUENCE - topInfluence);
    }

    private void DamageTaken(float _, IDamageable __)
    {
        float t = _health.HP / _health.MaxHP;
        Color colour = Color.Lerp(_botColour, _topColour, t);
        SetGradient(t, colour);
        OnDamaged?.Invoke(t, colour);
    }

    private void Died()
    {
        _sr.enabled = false;
    }

    private void OnResourceGot(ResourceBehaviour resource)
    {
        ModifyResource(resource.Type, 1);
        OnResourceAdded?.Invoke(resource.Type, 1);
        resource.Deactive();
    }
}
