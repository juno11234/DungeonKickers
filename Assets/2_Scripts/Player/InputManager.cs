using UnityEngine;

public class InputManager : MonoBehaviour
{
    PlayerInput input;
    PlayerInput.PlayerActionActions playerAction;
    ControlManager control;

    private void Awake()
    {
        input = new PlayerInput();
        playerAction = input.PlayerAction;
        control = FindAnyObjectByType<ControlManager>();

        playerAction.Move.performed += ctx => control.ShootRayRight();
        playerAction.LeftButton.performed += ctx => control.ShootRayLeft();
        playerAction.LeftShift.performed += ctx => control.InputShift = true;
        playerAction.LeftShift.canceled += ctx => control.InputShift = false;
        
    }

    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }
}