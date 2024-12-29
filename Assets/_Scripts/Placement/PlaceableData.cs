using UnityEngine;
using LTF.SerializedDictionary;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New_Placeable_Data", menuName = "SOs/Placeable Data")]
public class PlaceableData : ScriptableObject
{
    [SerializeField] private string _name;
    [SerializeField] private SerializedDictionary<ResourceType, int> _resourcesCost;

    [SerializeField] private Building _buildingPrefab;

    public string Name => _name;
    public Dictionary<ResourceType, int> ResourcesCost => _resourcesCost;

    public Building BuildingPrefab => _buildingPrefab;
}
