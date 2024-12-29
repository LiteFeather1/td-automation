using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField, TextArea(4, 4)] private string _text;

    public Action<string> OnHover { get; set; }
    public Action OnUnhover { get; set; }

    public void OnPointerEnter(PointerEventData _)
    {
        OnHover?.Invoke(_text);
    }

    public void OnPointerExit(PointerEventData _)
    {
        OnUnhover?.Invoke();
    }
}
