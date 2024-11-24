using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIBuildingButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private PlaceableData _placeableData;
    [SerializeField] private Image _image;
    [SerializeField] private Button _button;

    public Action<PlaceableData> OnButtonPressed { get; set; }
    public Action<UIBuildingButton> OnButtonHovered { get; set; }
    public Action OnButtonUnhovered { get; set; }

    public PlaceableData PlaceableData => _placeableData;
    public Dictionary<ResourceType, int> ResourceCost => _placeableData.BuildingPrefab.ResourceCost;

    public void B_ButtonPressed()
    {
        OnButtonPressed?.Invoke(_placeableData);
    }

    public void OnPointerEnter(PointerEventData _)
    {
        OnButtonHovered?.Invoke(this);
    }

    public void OnPointerExit(PointerEventData _)
    {
        OnButtonUnhovered?.Invoke();
    }

    public void SetButtonInteractable(bool interactable)
    {
        _button.interactable = interactable;
    }
}
