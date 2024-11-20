using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : Singleton<GameManager>
{
    [Header("Systems")]
    [SerializeField] private PlacementSystem _placementSystem;
    [SerializeField] private BeltPathSystem _beltPathSystem;
    [SerializeField] private GameHUD _gameHUD;

    public void OnEnable()
    {
        var inputs = InputManager.Instance.InputSystem.Player;
        inputs.LMB.performed += PlaceBuilding;
        inputs.RMB.performed += CancelBuilding;
        inputs.Rotate.performed += RotateBuilding;

        _placementSystem.OnBuildingPlaced += BuildingPlaced;

        foreach (var buildingButton in _gameHUD.UIBuildingButtons)
        {
            buildingButton.OnButtonPressed += _placementSystem.SetPlaceable;
        }
    }

    public void OnDisable()
    {
        var inputs = InputManager.Instance.InputSystem.Player;
        inputs.LMB.performed -= PlaceBuilding;
        inputs.RMB.performed -= CancelBuilding;
        inputs.Rotate.performed -= RotateBuilding;

        _placementSystem.OnBuildingPlaced -= BuildingPlaced;

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
        if (building is BeltPath beltPath)
        {
            _beltPathSystem.AddPoint(beltPath);
        }
    }
}
