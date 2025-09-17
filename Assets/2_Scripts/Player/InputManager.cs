using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class InputManager : MonoBehaviour
{
    private PlayerInput input;
    private PlayerInput.PlayerActionActions playerAction;

    // 마우스 왼쪽 클릭(선택) 이벤트를 외부에 노출합니다.
    public delegate void SelectActionHandler(Vector2 position);

    public event SelectActionHandler OnSelectAction;

    // 마우스 오른쪽 클릭(명령) 이벤트를 외부에 노출합니다.
    public delegate void MoveOrAttackActionHandler(Vector2 position);

    public event MoveOrAttackActionHandler OnMoveOrAttackAction;

    // Shift 키 상태를 외부에 노출합니다.
    public delegate void ShiftStatusChangedHandler(bool isShiftPressed);

    public event ShiftStatusChangedHandler OnShiftStatusChanged;

    private void Awake()
    {
        input = new PlayerInput();
        playerAction = input.PlayerAction;

        // Input System의 콜백을 Unity Event로 변환하여 외부에 알립니다.
        playerAction.LeftButton.performed += ctx => OnSelectAction.Invoke(Mouse.current.position.ReadValue());
        playerAction.Move.performed += ctx => OnMoveOrAttackAction.Invoke(Mouse.current.position.ReadValue());
        playerAction.LeftShift.performed += ctx => OnShiftStatusChanged.Invoke(true);
        playerAction.LeftShift.canceled += ctx => OnShiftStatusChanged.Invoke(false);
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