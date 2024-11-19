using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIResource : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI t_count;
    [SerializeField] private Image i_icon;
    [SerializeField] private Color _fadedColor = Color.white;

    public void SetCount(int count)
    {
        if (count == 0)
        {
            i_icon.color = _fadedColor;
            t_count.text = "";
            return;
        }

        if (i_icon.color.a != 1f)
            i_icon.color = Color.white;

        t_count.text = count.ToString();
    }
}
