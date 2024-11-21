using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBuildingButton : MonoBehaviour
{
    [SerializeField] private PlaceableData _placeableData;
    [SerializeField] private Image _image;
    [SerializeField] private Button _button;

    public Action<PlaceableData> OnButtonPressed { get; set; }

    // TODO Remove This
    public PlaceableData PlaceableData => _placeableData;
    public Dictionary<ResourceType, int> ResourceCost => _placeableData.BuildingPrefab.ResourceCost;

    public void B_ButtonPressed()
    {
        OnButtonPressed?.Invoke(_placeableData);
    }

    public void SetButtonInteractable(bool interactable)
    {
        _button.interactable = interactable;
    }
}
