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
    [SerializeField] private UIPauseScreen _pauseScreen;
    [SerializeField] private CameraManager _cameraManager;

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

        InputManager.Instance.InputSystem.UI.Cancel.performed += PauseUnpause;

        _placementSystem.OnBuildingPlaced += BuildingPlaced;
        _placementSystem.OnBuildingRemoved += BuildingRemoved;
        _placementSystem.OnResourceCollected += _factoryTower.AddResource;
        _placementSystem.OnHoverableHovered += _gameHUD.ShowHover;
        _placementSystem.OnHoverableUnhovered += _gameHUD.HideHover;

        _enemyManager.OnEnemyReachedPathEnd += _factoryTower.Health.TakeDamage;
        _enemyManager.OnEnemyKilled += EnemyKilled;
        _enemyManager.OnWaveStarted += WaveStarted;
        _enemyManager.OnStageEnded += _gameHUD.SetWave;
        _enemyManager.OnAllStagesEnded += GameEnded;

        _factoryTower.OnResourceModified += UpdateUIResources;
        _factoryTower.OnResourcesModified += _gameHUD.UpdateAmountsAndBuildingButtons;
        _factoryTower.OnResourceAdded += _endScreen.AddResource;
        _factoryTower.OnResourcesAdded += _endScreen.AddResources;
        _factoryTower.Health.OnDamageTaken += _gameHUD.UpdatePlayerHealth;
        _factoryTower.Health.OnHealed += _gameHUD.UpdatePlayerHealth;
        _factoryTower.Health.OnDied += GameEnded;

        _gameHUD.IncreaseSpeedButton.onClick.AddListener(IncreaseGameSpeed);
        _gameHUD.DecreaseSpeedButton.onClick.AddListener(DecreaseGameSpeed);

        _pauseScreen.OnResume.AddListener(Unpause);
        _pauseScreen.OnForfeit.AddListener(Forfeit);

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
        _gameHUD.SetGameSpeed(Time.timeScale);

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

        InputManager.Instance.InputSystem.UI.Cancel.performed -= PauseUnpause;

        _placementSystem.OnBuildingPlaced -= BuildingPlaced;
        _placementSystem.OnBuildingRemoved -= BuildingRemoved;
        _placementSystem.OnResourceCollected -= _factoryTower.AddResource;
        _placementSystem.OnHoverableHovered -= _gameHUD.ShowHover;
        _placementSystem.OnHoverableUnhovered -= _gameHUD.HideHover;

        _enemyManager.OnEnemyReachedPathEnd -= _factoryTower.Health.TakeDamage;
        _enemyManager.OnEnemyKilled -= EnemyKilled;
        _enemyManager.OnWaveStarted -= WaveStarted;
        _enemyManager.OnStageEnded -= _gameHUD.SetWave;
        _enemyManager.OnAllStagesEnded -= GameEnded;

        _factoryTower.OnResourceModified -= UpdateUIResources;
        _factoryTower.OnResourcesModified -= _gameHUD.UpdateAmountsAndBuildingButtons;
        _factoryTower.OnResourceAdded -= _endScreen.AddResource;
        _factoryTower.OnResourcesAdded -= _endScreen.AddResources;
        _factoryTower.Health.OnDamageTaken -= _gameHUD.UpdatePlayerHealth;
        _factoryTower.Health.OnHealed -= _gameHUD.UpdatePlayerHealth;
        _factoryTower.Health.OnDied -= GameEnded;

        _gameHUD.IncreaseSpeedButton.onClick.RemoveListener(IncreaseGameSpeed);
        _gameHUD.DecreaseSpeedButton.onClick.RemoveListener(DecreaseGameSpeed);

        _pauseScreen.OnResume.RemoveListener(Unpause);
        _pauseScreen.OnForfeit.RemoveListener(Forfeit);

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

    private void OnDestroy()
    {
        Time.timeScale = 1f;
        var inputManager = InputManager.Instance;
        inputManager.InputSystem.Player.Enable();
        inputManager.EnableInputSystem();
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

    private void Unpause()
    {
        Time.timeScale = _gameSpeeds[_gameSpeedIndex];
        InputManager.Instance.InputSystem.Player.Enable();
        _pauseScreen.SetContentState(false);
    }

    private void PauseUnpause()
    {
        if (Time.timeScale <= float.Epsilon)
        {
            Unpause();
        }
        else
        {
            Time.timeScale = 0f;
            InputManager.Instance.InputSystem.Player.Disable();
            _pauseScreen.SetContentState(true);
            _placementSystem.UnhoverResource();
        }
    }

    private void Forfeit()
    {
        _pauseScreen.SetContentState(false);
        _factoryTower.Health.TakeDamage(_factoryTower.Health.HP);
    }

    private void PauseUnpause(InputAction.CallbackContext _) => PauseUnpause();

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

    private void GameEnded()
    {
        Time.timeScale = 0f;

        InputManager.Instance.DisableInputSystem();

        _endScreen.Enable(_factoryTower.Health.HP > 0, _elapsedTime);
        _cameraManager.enabled = false;
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
