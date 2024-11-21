using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LTF.SerializedDictionary;

public class GameHUD : MonoBehaviour
{
    [Header("Waves")]
    [SerializeField] private TextMeshProUGUI t_currentWave;
    [SerializeField] private TextMeshProUGUI t_timeToWave;
    [SerializeField] private Image i_playerHealth;

    [Header("Resources")]
    [SerializeField] private TextMeshProUGUI t_towerCount;
    [SerializeField] private SerializedDictionary<ResourceType, UIResource> _uiResources;

    [Header("Building Buttons")]
    [SerializeField] private UIBuildingButton[] _uiBuildingButtons;

    public UIBuildingButton[] UIBuildingButtons => _uiBuildingButtons;

    public void UpdateUIResource(ResourceType resourceType, int amount)
    {
        _uiResources[resourceType].SetAmount(amount);
        UpdateBuildingButtons(resourceType, amount);
    }

    public void UpdatePlayerHealth(float _, IDamageable damageable)
    {
        i_playerHealth.fillAmount = damageable.HP / damageable.MaxHP;
    }

    public void SetWave(int wave)
    {
        t_currentWave.text = $"Wave {wave + 1}";
    }

    public void SetTimeToWave(float time)
    {
        t_timeToWave.text = $"{Mathf.FloorToInt(time / 60f):00}:{Mathf.FloorToInt(time % 60):00}";
    }

    public void SetTimeToWave(string text)
    {
        t_timeToWave.text = text;
    }

    public void UpdateAmountsAndBuildingButtons(Dictionary<ResourceType, int> resources)
    {
        foreach (var resource in resources)
        {
            UpdateUIResource(resource.Key, resource.Value);
            UpdateBuildingButtons(resource.Key, resource.Value);
        }
    }

    private void UpdateBuildingButtons(ResourceType type, int cost)
    {
        foreach (var button in _uiBuildingButtons)
        {
            if (button.PlaceableData == null
                || button.ResourceCost.Count == 0
                || !button.ResourceCost.ContainsKey(type))
                continue;

            var count = 0;
            foreach (var resourceCost in button.ResourceCost.Values)
            {
                if (-resourceCost <= cost)
                    count++;
            }

            button.SetButtonInteractable(count == button.ResourceCost.Count);
        }
    }
}
