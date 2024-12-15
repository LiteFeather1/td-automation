using System;
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
    [SerializeField] private UIHover _towerCount;
    [SerializeField] private SerializedDictionary<ResourceType, UIResource> _uiResources;

    [Header("Speedd")]
    [SerializeField] private TextMeshProUGUI t_gameSpeed;
    [SerializeField] private Button b_increaseSpeed;
    [SerializeField] private Button b_decreaseSpeed;

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

    public Button IncreaseSpeedButton => b_increaseSpeed;
    public Button DecreaseSpeedButton => b_decreaseSpeed;

    internal void OnEnable()
    {
        SubscribeToUIHover(_towerCount);

        foreach (var uiResource in _uiResources.Values)
        {
            SubscribeToUIHover(uiResource.UIHover);
        }

        foreach (var buildingButton in _uiBuildingButtons)
        {
            buildingButton.OnButtonHovered += BuildingButtonHovered;
            buildingButton.OnButtonUnhovered += BuildingButtonUnhovered;
        }

        void SubscribeToUIHover(UIHover uiHover)
        {
            uiHover.OnHover += ShowUIResourceDescription;
            uiHover.OnUnhover += HideHover;
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
        UnsubscribeToUIHover(_towerCount);

        foreach (var uiResource in _uiResources.Values)
        {
            UnsubscribeToUIHover(uiResource.UIHover);
        }

        foreach (var buildingButton in _uiBuildingButtons)
        {
            buildingButton.OnButtonHovered -= BuildingButtonHovered;
            buildingButton.OnButtonUnhovered -= BuildingButtonUnhovered;
        }

        void UnsubscribeToUIHover(UIHover uiHover)
        {
            uiHover.OnHover -= ShowUIResourceDescription;
            uiHover.OnUnhover -= HideHover;
        }
    }

    public void UpdateUIResource(ResourceType resourceType, int amount)
    {
        _uiResources[resourceType].SetAmount(amount);
    }

    public void SetGameSpeed(float speed)
    {
        if (speed % 1f <= float.Epsilon)
            t_gameSpeed.text = $"Speed {speed}x";
        else
            t_gameSpeed.text = $"Speed {speed:0.00}x";
    }

    public void UpdateBuildingButtons(
        ResourceType type, int totalAmount, IDictionary<ResourceType, int> resources
    )
    {
        foreach (var button in _uiBuildingButtons)
        {
            if (!button.ResourceCost.TryGetValue(type, out var cost) || -cost < totalAmount)
                continue;

            var count = button.ResourceCost.Count;
            foreach (var pair in button.ResourceCost)
            {
                if (-pair.Value <= resources[pair.Key])
                    count--;
            }

            button.SetButtonInteractable(count == 0);
        }
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

    public void UpdatePlayerHealth(float _, IDamageable damageable)
    {
        i_playerHealth.fillAmount = damageable.HP / damageable.MaxHP;
    }

    public void SetTowerCount(int count)
    {
        t_towerCount.text = count.ToString();
    }

    public void UpdateAmountsAndBuildingButtons(Dictionary<ResourceType, int> resources)
    {
        foreach (var pair in resources)
        {
            UpdateUIResource(pair.Key, pair.Value);
            UpdateBuildingButtons(pair.Key, pair.Value, resources);
        }
    }

    public void ShowHover(IHoverable hoverable)
    {
        ShowHover(hoverable.GetText());
    }

    public void HideHover()
    {
        _hoverBuildingInfo.gameObject.SetActive(false);
    }

    private void ShowHover(string text)
    {
        t_hoverBuilding.text = text;
        _hoverBuildingInfo.gameObject.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(_hoverBuildingInfo);
    }

    private void ShowUIResourceDescription(string description)
    {
        ShowHover(description);
    }

    private void BuildingButtonHovered(UIBuildingButton buildingButton)
    {
        var buildingRect = (RectTransform)buildingButton.transform;
        var (pivot, paddingDir) = (buildingRect.position.x > rt_canvas.rect.width * .5f) ?
            (1f, -1f) : (0, 1f);

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
