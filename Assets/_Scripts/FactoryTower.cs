using System;
using System.Collections.Generic;
using UnityEngine;

public class FactoryTower : MonoBehaviour
{
    [SerializeField] private Health _health;
    [SerializeField] private Receiver[] _receivers;

    private readonly Dictionary<ResourceType, int> r_resources = new();

    public delegate void ResourceAdded(ResourceType type, int totalAmount);
    public ResourceAdded OnResourceModified { get; set; }

    public Health Health => _health;

    public Receiver[] Receivers => _receivers;

    public void Awake()
    {
        foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
            r_resources.Add(type, 0);
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

    private void ModifyResource(ResourceType type, int amount)
    {
        r_resources[type] += amount;
        OnResourceModified?.Invoke(type, r_resources[type]);
    }

    private void OnResourceGot(ResourceBehaviour resource)
    {
        ModifyResource(resource.Type, 1);
        Destroy(resource.gameObject);
    }
}
