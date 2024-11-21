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

        _enemyManager.OnEnemyReachedPathEnd += _factoryTower.Health.TakeDamage;

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
    }

    public void OnDisable()
    {
        var inputs = InputManager.Instance.InputSystem.Player;
        inputs.LMB.performed -= PlaceBuilding;
        inputs.RMB.performed -= CancelBuilding;
        inputs.Rotate.performed -= RotateBuilding;

        _placementSystem.OnBuildingPlaced -= BuildingPlaced;

        _enemyManager.OnEnemyReachedPathEnd -= _factoryTower.Health.TakeDamage;

        foreach (var buildingButton in _gameHUD.UIBuildingButtons)
        {
            buildingButton.OnButtonPressed -= _placementSystem.SetPlaceable;
        }
    }

    private void PlaceBuilding(InputAction.CallbackContext _)
    {
        _placementSystem.PlaceBuilding();
    }

    private void CancelBuilding(InputAction.CallbackContext _)
    {
        _placementSystem.CancelBuilding();
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
}
