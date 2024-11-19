using UnityEngine;

[CreateAssetMenu(fileName = "New_Placeable_Data", menuName = "SOs/Placeable Data")]
public class PlaceableData : ScriptableObject
{
    [SerializeField] private string _name;
    [SerializeField, TextArea] private string _description;
    [SerializeField] private Sprite _icon;

    [SerializeField] private Building _buildingPrefab;

    // TODO Cost

    public string Name => _name;
    public string Description => _description;
    public Sprite Icon => _icon;

    public Building BuildingPrefab => _buildingPrefab;
}
