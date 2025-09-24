using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class InputManager : MonoBehaviour
{
    private PlayerInput input;
    private PlayerInput.PlayerActionActions playerAction;

    // 마우스 왼쪽 클릭(선택) 이벤트
    public delegate void SelectActionHandler(Vector2 position);

    public event SelectActionHandler OnSelectAction;
    public event SelectActionHandler OnSelectReleased;
    // 마우스 오른쪽 클릭(명령) 이벤트
    public event SelectActionHandler OnMoveOrAttackAction;

    // 누를때와 땔때를 감지할 때
    public delegate void ButtonPushPullHandler(bool isPressed);

    public event ButtonPushPullHandler OnShiftStatusChanged;
    public event ButtonPushPullHandler OnAKeyChanged;
    public event ButtonPushPullHandler OnMKeyChanged;

    //인덱스 입력
    public delegate void ButtonOlnyPushHandler(int index);

    public event ButtonOlnyPushHandler SelectGroup;
    public event ButtonOlnyPushHandler AddGroup;
    //마우스 스크롤
    public delegate void ScrollHandler(float scrollValue);

    public event ScrollHandler OnScrollInput;

    private void Awake()
    {
        input = new PlayerInput();
        playerAction = input.PlayerAction;

        // Input System의 콜백을 Unity Event로 변환하여 외부에 알립니다.
        playerAction.LeftButton.performed += ctx => OnSelectAction?.Invoke(Mouse.current.position.ReadValue());
        playerAction.LeftButton.canceled += ctx => OnSelectReleased?.Invoke(Mouse.current.position.ReadValue());

        playerAction.Move.performed += ctx => OnMoveOrAttackAction?.Invoke(Mouse.current.position.ReadValue());

        playerAction.LeftShift.performed += ctx => OnShiftStatusChanged?.Invoke(true);
        playerAction.LeftShift.canceled += ctx => OnShiftStatusChanged?.Invoke(false);

        playerAction.Zoom.performed += ctx => OnScrollInput?.Invoke(ctx.ReadValue<float>());
        playerAction.Attack.performed += ctx => OnAKeyChanged.Invoke(true);
        playerAction.M.performed += ctx => OnMKeyChanged.Invoke(true);
     
        playerAction.Select1.performed += ctx =>
        {
            var control = ctx.control;
            if (control.displayName == "1")
                SelectGroup?.Invoke(0);
            else if (control.displayName == "2")
                SelectGroup?.Invoke(1);
            else if (control.displayName == "3")
                SelectGroup?.Invoke(2);
            else if (control.displayName == "4")
                SelectGroup?.Invoke(3);
        };

        playerAction.AddGroup.performed += ctx =>
        {
            var control = ctx.control;
            if (control.displayName == "1")
                AddGroup?.Invoke(0);
            else if (control.displayName == "2")
                AddGroup?.Invoke(1);
            else if (control.displayName == "3")
                AddGroup?.Invoke(2);
            else if (control.displayName == "4")
                AddGroup?.Invoke(3);
        };

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