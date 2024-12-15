using UnityEngine;
using UnityEngine.InputSystem;
using LTF.SerializedDictionary;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private FactoryTower _factoryTower;

    [Header("Game Speed")]
    [SerializeField] private float[] _gameSpeeds = new float[] { .25f, .5f, .75f, 1f, 1.25f, 1.5f, 2f };
    private int _gameSpeedIndex = 3;

    [Header("Systems")]
    [SerializeField] private PlacementSystem _placementSystem;
    [SerializeField] private EnemyManager _enemyManager;
    [SerializeField] private GameHUD _gameHUD;
    [SerializeField] private UIEndScreen _endScreen;

    [Header("Misc")]
    [SerializeField] private SerializedDictionary<ResourceType, SOObjectPoolResourceBehaviour> _poolResources;

    private float _elapsedTime;

    private int _towerCount;

    public FactoryTower TowerFactory => _factoryTower;

    internal void OnEnable()
    {
        var inputs = InputManager.Instance.InputSystem.Player;
        inputs.LMB.performed += PlaceBuilding;
        inputs.RMB.performed += CancelBuilding;
        inputs.Rotate.performed += RotateBuilding;
        inputs.SpeedUp.performed += SpeedUp;
        inputs.SpeedDown.performed += SpeedDown;

        _placementSystem.OnBuildingPlaced += BuildingPlaced;
        _placementSystem.OnBuildingRemoved += BuildingRemoved;
        _placementSystem.OnResourceCollected += _factoryTower.AddResource;
        _placementSystem.OnHoverableHovered += _gameHUD.ShowHover;
        _placementSystem.OnHoverableUnhovered += _gameHUD.HideHover;

        _enemyManager.OnEnemyReachedPathEnd += _factoryTower.Health.TakeDamage;
        _enemyManager.OnEnemyKilled += EnemyKilled;
        _enemyManager.OnWaveStarted += WaveStarted;
        _enemyManager.OnStageEnded += _gameHUD.SetWave;
        _enemyManager.OnAllStagesEnded += AllStagesEnded;

        _factoryTower.OnResourceModified += UpdateUIResources;
        _factoryTower.OnResourcesModified += _gameHUD.UpdateAmountsAndBuildingButtons;
        _factoryTower.OnResourceAdded += _endScreen.AddResource;
        _factoryTower.OnResourcesAdded += _endScreen.AddResources;
        _factoryTower.Health.OnDamageTaken += _gameHUD.UpdatePlayerHealth;
        _factoryTower.Health.OnHealed += _gameHUD.UpdatePlayerHealth;

        _gameHUD.IncreaseSpeedButton.onClick.AddListener(IncreaseGameSpeed);
        _gameHUD.DecreaseSpeedButton.onClick.AddListener(DecreaseGameSpeed);

        foreach (var buildingButton in _gameHUD.UIBuildingButtons)
        {
            buildingButton.OnButtonPressed += _placementSystem.SetPlaceable;
        }

        foreach (var pool in _poolResources.Values)
        {
            pool.ObjectPool.ObjectCreated += ResourceCreated;
            pool.ObjectPool.InitPool();
        }
    }

    internal void Start()
    {
        AddBuildings(_factoryTower.Receivers);
        foreach (var receiver in _factoryTower.Receivers)
        {
            _placementSystem.BeltPathSystem.AddInPort(receiver);
        }
        AddBuildings(_factoryTower.StarterTowers);

        _gameHUD.SetWave(_enemyManager.CurrentStage);
        _gameHUD.SetTowerCount(_towerCount = _factoryTower.StarterTowers.Length);
        _gameHUD.SetGameSpeed(_gameSpeeds[_gameSpeedIndex]);

        _endScreen.Init();

        void AddBuildings(Building[] buildings)
        {
            foreach (var building in buildings)
            {
                building.Position = Vector2Int.FloorToInt(building.transform.position);
                _placementSystem.AddBuilding(building);
            }
        }
    }

    internal void Update()
    {
        _elapsedTime += Time.deltaTime;

        if (!_enemyManager.AllStagesCompleted && !_enemyManager.WaveInProgress)
            _gameHUD.SetTimeToWave(_enemyManager.TimeToWave);
    }

    internal void OnDisable()
    {
        var inputs = InputManager.Instance.InputSystem.Player;
        inputs.LMB.performed -= PlaceBuilding;
        inputs.RMB.performed -= CancelBuilding;
        inputs.Rotate.performed -= RotateBuilding;
        inputs.SpeedUp.performed -= SpeedUp;
        inputs.SpeedDown.performed -= SpeedDown;

        _placementSystem.OnBuildingPlaced -= BuildingPlaced;
        _placementSystem.OnBuildingRemoved -= BuildingRemoved;
        _placementSystem.OnResourceCollected -= _factoryTower.AddResource;
        _placementSystem.OnHoverableHovered -= _gameHUD.ShowHover;
        _placementSystem.OnHoverableUnhovered -= _gameHUD.HideHover;

        _enemyManager.OnEnemyReachedPathEnd -= _factoryTower.Health.TakeDamage;
        _enemyManager.OnEnemyKilled -= EnemyKilled;
        _enemyManager.OnWaveStarted -= WaveStarted;
        _enemyManager.OnStageEnded -= _gameHUD.SetWave;
        _enemyManager.OnAllStagesEnded -= AllStagesEnded;

        _factoryTower.OnResourceModified -= UpdateUIResources;
        _factoryTower.OnResourcesModified -= _gameHUD.UpdateAmountsAndBuildingButtons;
        _factoryTower.OnResourceAdded -= _endScreen.AddResource;
        _factoryTower.OnResourcesAdded -= _endScreen.AddResources;
        _factoryTower.Health.OnDamageTaken -= _gameHUD.UpdatePlayerHealth;
        _factoryTower.Health.OnHealed -= _gameHUD.UpdatePlayerHealth;

        _gameHUD.IncreaseSpeedButton.onClick.RemoveListener(IncreaseGameSpeed);
        _gameHUD.DecreaseSpeedButton.onClick.RemoveListener(DecreaseGameSpeed);

        foreach (var buildingButton in _gameHUD.UIBuildingButtons)
        {
            buildingButton.OnButtonPressed -= _placementSystem.SetPlaceable;
        }

        foreach (var pool in _poolResources.Values)
        {
            pool.ObjectPool.ObjectCreated -= ResourceCreated;
            foreach (var resource in pool.ObjectPool.Objects)
            {
                resource.OnReturnToPool -= pool.ObjectPool.ReturnObject;
            }
            pool.ObjectPool.Dispose();
        }
    }

    private void PlaceBuilding(InputAction.CallbackContext _)
    {
        _placementSystem.LeftClick();
    }

    private void CancelBuilding(InputAction.CallbackContext _)
    {
        _placementSystem.TryCancelOrDesconstructBuilding();
    }

    private void RotateBuilding(InputAction.CallbackContext _)
    {
        _placementSystem.RotateBuilding();
    }

    private void ChangeGameSpeed(float speed)
    {
        Time.timeScale = speed;
        _gameHUD.SetGameSpeed(speed);
        _gameHUD.IncreaseSpeedButton.interactable = _gameSpeedIndex != _gameSpeeds.Length - 1;
        _gameHUD.DecreaseSpeedButton.interactable = _gameSpeedIndex != 0;
    }

    private void IncreaseGameSpeed()
    {
        _gameSpeedIndex = Mathf.Min(++_gameSpeedIndex, _gameSpeeds.Length - 1);
        ChangeGameSpeed(_gameSpeeds[_gameSpeedIndex]);
    }

    private void SpeedUp(InputAction.CallbackContext _) => IncreaseGameSpeed();

    private void DecreaseGameSpeed()
    {
        _gameSpeedIndex = Mathf.Max(--_gameSpeedIndex, 0);
        ChangeGameSpeed(_gameSpeeds[_gameSpeedIndex]);
    }

    private void SpeedDown(InputAction.CallbackContext _) => DecreaseGameSpeed();

    public void BuildingPlaced(Building building)
    {
        if (building is Tower)
        {
            _gameHUD.SetTowerCount(++_towerCount);
        }

        _factoryTower.RemoveResources(building.ResourceCost);

        if (!_factoryTower.HasEnoughResourceToBuild(building.ResourceCost))
            _placementSystem.UnselectBuildingBuilding();
    }

    public void BuildingRemoved(Building building)
    {
        _factoryTower.AddResources(building.ResourceCost);

        if (building is Tower)
        {
            _gameHUD.SetTowerCount(--_towerCount);
        }
    }

    private void EnemyKilled()
    {
        _endScreen.AddEnemyKilled();
        _factoryTower.AddResource(ResourceType.Essence);
    }

    private void WaveStarted()
    {
        _gameHUD.SetTimeToWave("Defend!");
    }

    private void AllStagesEnded()
    {
        _endScreen.Enable(_factoryTower.Health.HP > 0, _elapsedTime);
        Time.timeScale = 0f;
    }

    private void UpdateUIResources(ResourceType type, int amount)
    {
        _gameHUD.UpdateUIResource(type, amount);
        _gameHUD.UpdateBuildingButtons(type, amount, _factoryTower.Resources);
    }

    private void ResourceCreated(ResourceBehaviour resource)
    {
        resource.OnReturnToPool += _poolResources[resource.Type].ObjectPool.ReturnObject;
    }
}
