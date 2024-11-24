using UnityEngine;

[CreateAssetMenu(fileName = "New_Placeable_Data", menuName = "SOs/Placeable Data")]
public class PlaceableData : ScriptableObject
{
    [SerializeField, TextArea] private string _description;
    [SerializeField] private Sprite _icon;

    [SerializeField] private Building _buildingPrefab;

    public string Name => _buildingPrefab.Name;
    public string Description => _description;
    public Sprite Icon => _icon;

    public Building BuildingPrefab => _buildingPrefab;
}
