using UnityEngine;
using UnityEngine.InputSystem;
using LTF.SerializedDictionary;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private FactoryTower _factoryTower;

    [Header("Systems")]
    [SerializeField] private PlacementSystem _placementSystem;
    [SerializeField] private BeltPathSystem _beltPathSystem;
    [SerializeField] private EnemyManager _enemyManager;
    [SerializeField] private GameHUD _gameHUD;
    [SerializeField] private UIEndScreen _endScreen;

    [Header("Misc")]
    [SerializeField] private SerializedDictionary<ResourceType, SOObjectPoolResourceBehaviour> _poolResources;

    private float _elapsedTime;

    public FactoryTower TowerFactory => _factoryTower;

    internal void OnEnable()
    {
        var inputs = InputManager.Instance.InputSystem.Player;
        inputs.LMB.performed += PlaceBuilding;
        inputs.RMB.performed += CancelBuilding;
        inputs.Rotate.performed += RotateBuilding;

        _placementSystem.OnBuildingPlaced += BuildingPlaced;
        _placementSystem.OnBuildingRemoved += BuildingRemoved;
        _placementSystem.OnResourceCollected += _factoryTower.AddResource;
        _placementSystem.OnHoverableHovered += _gameHUD.SetHoverBuilding;
        _placementSystem.OnHoverableUnhovered += _gameHUD.UnhoverBuilding;

        _enemyManager.OnEnemyReachedPathEnd += _factoryTower.Health.TakeDamage;
        _enemyManager.OnEnemyKilled += _endScreen.AddEnemyKilled;
        _enemyManager.OnWaveStarted += WaveStarted;
        _enemyManager.OnStageEnded += _gameHUD.SetWave;
        _enemyManager.OnAllStagesEnded += AllStagesEnded;

        _factoryTower.OnResourceModified += _gameHUD.UpdateUIResource;
        _factoryTower.OnResourcesModified += _gameHUD.UpdateAmountsAndBuildingButtons;
        _factoryTower.OnResourceAdded += _endScreen.AddResource;
        _factoryTower.OnResourcesAdded += _endScreen.AddResources;
        _factoryTower.Health.OnDamageTaken += _gameHUD.UpdatePlayerHealth;
        _factoryTower.Health.OnHealed += _gameHUD.UpdatePlayerHealth;

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
        AddBuildings(_factoryTower.StarterTowers);

        _gameHUD.SetWave(_enemyManager.CurrentStage);

        void AddBuildings(Building[] buildings)
        {
            foreach (var building in buildings)
            {
                building.Position = Vector2Int.FloorToInt(building.transform.position);
                _placementSystem.AddBuildingRaw(building);
            }
        }

        _endScreen.Init();
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

        _placementSystem.OnBuildingPlaced -= BuildingPlaced;
        _placementSystem.OnBuildingRemoved -= BuildingRemoved;
        _placementSystem.OnResourceCollected -= _factoryTower.AddResource;
        _placementSystem.OnHoverableHovered -= _gameHUD.SetHoverBuilding;
        _placementSystem.OnHoverableUnhovered -= _gameHUD.UnhoverBuilding;

        _enemyManager.OnEnemyReachedPathEnd -= _factoryTower.Health.TakeDamage;
        _enemyManager.OnEnemyKilled -= _endScreen.AddEnemyKilled;
        _enemyManager.OnWaveStarted -= WaveStarted;
        _enemyManager.OnStageEnded -= _gameHUD.SetWave;
        _enemyManager.OnAllStagesEnded -= AllStagesEnded;

        _factoryTower.OnResourceModified -= _gameHUD.UpdateUIResource;
        _factoryTower.OnResourcesModified -= _gameHUD.UpdateAmountsAndBuildingButtons;
        _factoryTower.OnResourceAdded -= _endScreen.AddResource;
        _factoryTower.OnResourcesAdded -= _endScreen.AddResources;
        _factoryTower.Health.OnDamageTaken -= _gameHUD.UpdatePlayerHealth;
        _factoryTower.Health.OnHealed -= _gameHUD.UpdatePlayerHealth;

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
        if (!UIMouseBlocker.MouseBlocked)
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

    public void BuildingPlaced(Building building)
    {
        if (building is IInPort inPort)
        {
            _beltPathSystem.AddIInPort(inPort);
        }

        if (building is IOutPort outPort)
        {
            _beltPathSystem.AddOutPort(outPort);
        }

        _factoryTower.RemoveResources(building.ResourceCost);

        if (!_factoryTower.HasEnoughResourceToBuild(building.ResourceCost))
            _placementSystem.UnselectBuildingBuilding();
    }

    public void BuildingRemoved(Building building)
    {
        _beltPathSystem.TryRemovePosition(building.Position);

        _factoryTower.AddResources(building.ResourceCost);
    }

    private void WaveStarted()
    {
        _gameHUD.SetTimeToWave("Attacking");
    }

    private void AllStagesEnded()
    {
        _endScreen.Enable(_factoryTower.Health.HP > 0, _elapsedTime);
        Time.timeScale = 0f;
    }

    private void ResourceCreated(ResourceBehaviour resource)
    {
        resource.OnReturnToPool += _poolResources[resource.Type].ObjectPool.ReturnObject;
    }
}
