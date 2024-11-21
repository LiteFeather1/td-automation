using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIResource : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI t_count;
    [SerializeField] private Image i_icon;
    [SerializeField] private Color _normalColour = Color.white;
    [SerializeField] private Color _fadedColor = Color.white;

    public void SetAmount(int count)
    {
        if (count == 0)
        {
            i_icon.color = _fadedColor;
            t_count.text = "";
        }
        else if (i_icon.color.a != 1f)
            i_icon.color = _normalColour;

        t_count.text = count.ToString();
    }
}
