using UnityEngine;
using TMPro;

public class UIEndScreenStat : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI t_stat;

    public void SetStat(string text) => t_stat.text = text;
}
