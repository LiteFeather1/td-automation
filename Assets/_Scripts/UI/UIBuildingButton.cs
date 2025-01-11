using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TMPro;

public class UIBuildingButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private static readonly Color sr_fadedColour = new(1f, 1f, 1f, .125f);

    [SerializeField] private Building _buildingPrefab;
    [SerializeField] private Image i_icon;
    [SerializeField] private Button _button;
    [SerializeField] private Image i_inputBackground;
    [SerializeField] private TextMeshProUGUI t_inputName;

    [SerializeField] private InputAction _pressInputAction;

    public Action<Building> OnButtonPressed { get; set; }
    public Action<UIBuildingButton> OnButtonHovered { get; set; }
    public Action OnButtonUnhovered { get; set; }

    public Building Building => _buildingPrefab;

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
        OnButtonPressed?.Invoke(_buildingPrefab);
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
        if (interactable)
        {
            i_icon.color = Color.white;
            t_inputName.alpha = 1f;
        }
        else
        {
            i_icon.color = sr_fadedColour;
            t_inputName.alpha = .125f;
        }
    }

    private void PressInput(InputAction.CallbackContext _)
    {
        if (_button.interactable)
            OnButtonPressed?.Invoke(_buildingPrefab);
    }
}
