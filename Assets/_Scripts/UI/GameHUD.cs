using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameHUD : MonoBehaviour
{
    [Header("Waves")]
    [SerializeField] private TextMeshProUGUI t_currentWave;
    [SerializeField] private TextMeshProUGUI t_timeToWave;
    [SerializeField] private Image i_playerHealth;

    [Header("Resources")]
    [SerializeField] private TextMeshProUGUI t_towerCount;
    [SerializeField] private UIResource _woodResource;
    [SerializeField] private UIResource _metalResource;
    [SerializeField] private UIResource _woodPlankResource;
    [SerializeField] private UIResource _metalPlateResource;
    [SerializeField] private UIResource _essenceResource;

    [Header("Building Buttons")]
    [SerializeField] private UIBuildingButton[] _uiBuildingButtons;

    public UIBuildingButton[] UIBuildingButtons => _uiBuildingButtons;
}
