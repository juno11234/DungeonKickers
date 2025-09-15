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

        playerAction.Move.performed += ctx => control.ShootRay();
        
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
