﻿using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UIResource : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField, TextArea] private string _resourceDescription;
    [SerializeField] private TextMeshProUGUI t_count;
    [SerializeField] private Image i_icon;
    [SerializeField] private Color _normalColour = Color.white;
    [SerializeField] private Color _fadedColor = Color.white;

    public Action<string> OnMouseHover { get; set; }
    public Action OnMouseUnhover { get; set; }

    public void SetAmount(int count)
    {
        if (count == 0)
        {
            i_icon.color = _fadedColor;
            t_count.text = "";
            return;
        }

        if (i_icon.color.a != 1f)
            i_icon.color = _normalColour;

        t_count.text = count.ToString();
    }

    public void OnPointerEnter(PointerEventData _)
    {
        OnMouseHover?.Invoke(_resourceDescription);
    }

    public void OnPointerExit(PointerEventData _)
    {
        OnMouseUnhover?.Invoke();
    }
}
