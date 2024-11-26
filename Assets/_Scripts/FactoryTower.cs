using System;
using System.Collections.Generic;
using UnityEngine;

public class FactoryTower : MonoBehaviour
{
    [SerializeField] private Health _health;
    [SerializeField] private Receiver[] _receivers;
    [SerializeField] private Tower[] _starterTowers;

    private readonly Dictionary<ResourceType, int> r_resources = new();

    public delegate void ResourceAdded(ResourceType type, int totalAmount);
    public ResourceAdded OnResourceModified { get; set; }

    public Action<Dictionary<ResourceType, int>> OnResourcesModified { get; set; }
    public ResourceAdded OnResourceAdded { get; set; }
    public Action<Dictionary<ResourceType, int>> OnResourcesAdded { get; set; }

    public Health Health => _health;

    public Receiver[] Receivers => _receivers;
    public Tower[] StarterTowers => _starterTowers;

    public void Awake()
    {
        foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
            r_resources.Add(type, 0);

        foreach (var tower in _starterTowers)
            foreach (var cost in tower.ResourceCost)
                r_resources[cost.Key] -= cost.Value;
    }

    public void OnEnable()
    {
        foreach (var receiver in _receivers)
        {
            receiver.OnResourceGot += OnResourceGot;
        }
    }

    public void OnDisable()
    {
        foreach (var receiver in _receivers)
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
        foreach (var resource in resources)
        {
            r_resources[resource.Key] += resource.Value;
        }

        OnResourcesModified?.Invoke(r_resources);
    }

    public void AddResources(Dictionary<ResourceType, int> resources)
    {
        foreach (var resource in resources)
        {
            r_resources[resource.Key] -= Mathf.FloorToInt(resource.Value * .5f);
        }

        OnResourcesModified?.Invoke(r_resources);
        OnResourcesAdded?.Invoke(r_resources);
    }

    public bool HasEnoughResourceToBuild(IDictionary<ResourceType, int> resourceCosts)
    {
        foreach (var resourceCost in resourceCosts)
            if (r_resources[resourceCost.Key] < -resourceCost.Value)
                return false;

        return true;
    }

    private void OnResourceGot(ResourceBehaviour resource)
    {
        ModifyResource(resource.Type, 1);
        OnResourceAdded?.Invoke(resource.Type, 1);
        resource.Deactive();
    }
}
