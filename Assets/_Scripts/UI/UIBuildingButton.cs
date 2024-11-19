using System;
using UnityEngine;
using UnityEngine.UI;

public class UIBuildingButton : MonoBehaviour
{
    [SerializeField] private PlaceableData _placeableData;
    [SerializeField] private Image _image;
    [SerializeField] private Button _button;

    public Action<PlaceableData> OnButtonPressed { get; set; }

    public void B_ButtonPressed()
    {
        OnButtonPressed?.Invoke(_placeableData);
    }
}
