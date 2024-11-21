using System;
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

    public void OnEnable()
    {
        var inputs = InputManager.Instance.InputSystem.Player;
        inputs.LMB.performed += PlaceBuilding;
        inputs.RMB.performed += CancelBuilding;
        inputs.Rotate.performed += RotateBuilding;

        _placementSystem.OnBuildingPlaced += BuildingPlaced;
        _placementSystem.OnBuildingRemoved += _beltPathSystem.TryRemovePosition;

        _enemyManager.OnEnemyReachedPathEnd += _factoryTower.Health.TakeDamage;
        _enemyManager.OnWaveStarted += WaveStarted;
        _enemyManager.OnStageEnded += _gameHUD.SetWave;

        _factoryTower.OnResourceModified += _gameHUD.UpdateUIResource;
        _factoryTower.Health.OnDamageTaken += _gameHUD.UpdatePlayerHealth;
        _factoryTower.Health.OnHealed -= _gameHUD.UpdatePlayerHealth;

        foreach (var buildingButton in _gameHUD.UIBuildingButtons)
        {
            buildingButton.OnButtonPressed += _placementSystem.SetPlaceable;
        }
    }

    protected override void Awake()
    {
        foreach (var receiver in _factoryTower.Receivers)
        {
            _placementSystem.AddBuilding(receiver);
        }
    }

    public void Start()
    {
        _gameHUD.SetWave(_enemyManager.CurrentStage);
    }

    public void Update()
    {
        if (!_enemyManager.WaveInProgress)
            _gameHUD.SetTimeToWave(_enemyManager.TimeToWave);
    }

    public void OnDisable()
    {
        var inputs = InputManager.Instance.InputSystem.Player;
        inputs.LMB.performed -= PlaceBuilding;
        inputs.RMB.performed -= CancelBuilding;
        inputs.Rotate.performed -= RotateBuilding;

        _placementSystem.OnBuildingPlaced -= BuildingPlaced;
        _placementSystem.OnBuildingRemoved -= _beltPathSystem.TryRemovePosition;

        _enemyManager.OnEnemyReachedPathEnd -= _factoryTower.Health.TakeDamage;
        _enemyManager.OnWaveStarted -= WaveStarted;
        _enemyManager.OnStageEnded -= _gameHUD.SetWave;

        _factoryTower.OnResourceModified -= _gameHUD.UpdateUIResource;
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
            _placementSystem.TryPlaceBuilding();
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
    }

    public void WaveStarted()
    {
        _gameHUD.SetTimeToWave("Attacking");
    }
}
