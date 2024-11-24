using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LTF.SerializedDictionary;

public class GameHUD : MonoBehaviour
{
    [SerializeField] private RectTransform rt_canvas;

    [Header("Waves")]
    [SerializeField] private TextMeshProUGUI t_currentWave;
    [SerializeField] private TextMeshProUGUI t_timeToWave;
    [SerializeField] private Image i_playerHealth;

    [Header("Resources")]
    [SerializeField] private TextMeshProUGUI t_towerCount;
    [SerializeField] private SerializedDictionary<ResourceType, UIResource> _uiResources;

    [Header("Building Buttons")]
    [SerializeField] private UIBuildingButton[] _uiBuildingButtons;

    [Header("Hover Building Button Info")]
    [SerializeField] private RectTransform _hoverBuildingButtonInfo;
    [SerializeField] private TextMeshProUGUI t_hoverBuildingInfoName;
    [SerializeField] private float _hoverBuildingButtonPadding = 32f;
    [SerializeField] private Color _hasEnoughResourceColour;
    [SerializeField] private Color _notEnoughResourceColour;
    [SerializeField] private SerializedDictionary<ResourceType, HoverBuildingCost> _hoverInfoCost;


    [Header("Hover Building")]
    [SerializeField] private RectTransform _hoverBuildingInfo;
    [SerializeField] private Vector3 _hoverBuildingPadding = new(16f, 16f);
    [SerializeField] private TextMeshProUGUI t_hoverBuilding;

    public UIBuildingButton[] UIBuildingButtons => _uiBuildingButtons;

    internal void OnEnable()
    {
        foreach (var buildingButton in _uiBuildingButtons)
        {
            buildingButton.OnButtonHovered += BuildingButtonHovered;
            buildingButton.OnButtonUnhovered += BuildingButtonUnhovered;
        }
    }

    internal void Update()
    {
        if (_hoverBuildingInfo.gameObject.activeSelf)
        {
            _hoverBuildingInfo.position = Input.mousePosition + _hoverBuildingPadding;
        }
    }

    internal void OnDisable()
    {
        foreach (var buildingButton in _uiBuildingButtons)
        {
            buildingButton.OnButtonHovered -= BuildingButtonHovered;
            buildingButton.OnButtonUnhovered -= BuildingButtonUnhovered;
        }
    }

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

    public void SetHoverBuilding(IHoverable hoverable)
    {
        t_hoverBuilding.text = hoverable.GetText();
        _hoverBuildingInfo.gameObject.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(_hoverBuildingInfo);
    }

    public void UnhoverBuilding()
    {
        _hoverBuildingInfo.gameObject.SetActive(false);
    }

    private void BuildingButtonHovered(UIBuildingButton buildingButton)
    {
        var buildingRect = (RectTransform)buildingButton.transform;
        float pivot;
        float paddingDir;
        if (buildingRect.position.x > rt_canvas.rect.width * .5f)
        {
            pivot = 1f;
            paddingDir = -1f;
        }
        else
        {
            pivot = 0;
            paddingDir = 1f;
        }
        _hoverBuildingButtonInfo.pivot = new(pivot, .5f);
        _hoverBuildingButtonInfo.position = new(
            buildingRect.position.x + ((buildingRect.rect.width + _hoverBuildingButtonPadding) * paddingDir),
            buildingRect.position.y
        );

        t_hoverBuildingInfoName.text = buildingButton.PlaceableData.Name;

        foreach (var resourceType in _hoverInfoCost)
        {
            var hoverInfoCost = _hoverInfoCost[resourceType.Key];
            var hasResource = buildingButton.PlaceableData.BuildingPrefab.ResourceCost.
                TryGetValue(resourceType.Key, out var cost);
            hoverInfoCost.Root.SetActive(hasResource);

            if (!hasResource)
                continue;

            hoverInfoCost.CountText.text = $"{-cost:00}";
            if (GameManager.Instance.TowerFactory.GetResourceAmount(resourceType.Key) >= -cost)
            {
                hoverInfoCost.CountText.color = _hasEnoughResourceColour;
                hoverInfoCost.Image.color = Color.white;
            }
            else
            {
                hoverInfoCost.CountText.color = _notEnoughResourceColour;
                hoverInfoCost.Image.color = new(1f, 1f, 1f, .5f);
            }
        }

        _hoverBuildingButtonInfo.gameObject.SetActive(true);

         // Idk unity resize problem
        LayoutRebuilder.ForceRebuildLayoutImmediate(_hoverBuildingButtonInfo);
    }

    private void BuildingButtonUnhovered()
    {
        _hoverBuildingButtonInfo.gameObject.SetActive(false);
    }

    [System.Serializable]
    private struct HoverBuildingCost
    {
        public GameObject Root;
        public Image Image;
        public TextMeshProUGUI CountText;
    }
}
