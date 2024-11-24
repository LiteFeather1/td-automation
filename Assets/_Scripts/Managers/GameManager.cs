using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private FactoryTower _factoryTower;

    [Header("Systems")]
    [SerializeField] private PlacementSystem _placementSystem;
    [SerializeField] private BeltPathSystem _beltPathSystem;
    [SerializeField] private EnemyManager _enemyManager;
    [SerializeField] private GameHUD _gameHUD;

    public FactoryTower TowerFactory => _factoryTower;

    public void OnEnable()
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
        _enemyManager.OnWaveStarted += WaveStarted;
        _enemyManager.OnStageEnded += _gameHUD.SetWave;

        _factoryTower.OnResourceModified += _gameHUD.UpdateUIResource;
        _factoryTower.OnResourcesModified += _gameHUD.UpdateAmountsAndBuildingButtons;
        _factoryTower.Health.OnDamageTaken += _gameHUD.UpdatePlayerHealth;
        _factoryTower.Health.OnHealed += _gameHUD.UpdatePlayerHealth;

        foreach (var buildingButton in _gameHUD.UIBuildingButtons)
        {
            buildingButton.OnButtonPressed += _placementSystem.SetPlaceable;
        }
    }

    public void Start()
    {
        foreach (var receiver in _factoryTower.Receivers)
        {
            _placementSystem.AddBuilding(receiver);
        }

        _gameHUD.SetWave(_enemyManager.CurrentStage);
    }

    public void Update()
    {
        if (!_enemyManager.AllStagesCompleted && !_enemyManager.WaveInProgress)
            _gameHUD.SetTimeToWave(_enemyManager.TimeToWave);
    }

    public void OnDisable()
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
        _enemyManager.OnWaveStarted -= WaveStarted;
        _enemyManager.OnStageEnded -= _gameHUD.SetWave;

        _factoryTower.OnResourceModified -= _gameHUD.UpdateUIResource;
        _factoryTower.OnResourcesModified -= _gameHUD.UpdateAmountsAndBuildingButtons;
        _factoryTower.Health.OnDamageTaken -= _gameHUD.UpdatePlayerHealth;
        _factoryTower.Health.OnHealed -= _gameHUD.UpdatePlayerHealth;
        foreach (var buildingButton in _gameHUD.UIBuildingButtons)
        {
            buildingButton.OnButtonPressed -= _placementSystem.SetPlaceable;
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

        _factoryTower.ModifyResources(building.ResourceCost);
    }

    public void BuildingRemoved(Building building)
    {
        _beltPathSystem.TryRemovePosition(building.Position);

        _factoryTower.DeconstructResources(building.ResourceCost);
    }

    public void WaveStarted()
    {
        _gameHUD.SetTimeToWave("Attacking");
    }
}
