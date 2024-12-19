using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TMPro;

public class UIBuildingButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private PlaceableData _placeableData;
    [SerializeField] private Image _image;
    [SerializeField] private Button _button;
    [SerializeField] private Image i_inputBackground;
    [SerializeField] private TextMeshProUGUI t_inputName;

    [SerializeField] private InputAction _pressInputAction;

    public Action<PlaceableData> OnButtonPressed { get; set; }
    public Action<UIBuildingButton> OnButtonHovered { get; set; }
    public Action OnButtonUnhovered { get; set; }

    public PlaceableData PlaceableData => _placeableData;
    public Dictionary<ResourceType, int> ResourceCost => _placeableData.BuildingPrefab.ResourceCost;

    internal void OnEnable()
    {
        _pressInputAction.performed += PressInput;
        _pressInputAction.Enable();
    }

    private IEnumerator Start()
    {
        t_inputName.text = _pressInputAction.GetBindingDisplayString();
        yield return null;
        i_inputBackground.rectTransform.sizeDelta = t_inputName.rectTransform.sizeDelta;
    }

    internal void OnDisable()
    {
        _pressInputAction.performed -= PressInput;
        _pressInputAction.Disable();
    }

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

    private void PressInput(InputAction.CallbackContext _)
    {
        if (_button.interactable)
            OnButtonPressed?.Invoke(_placeableData);
    }
}
