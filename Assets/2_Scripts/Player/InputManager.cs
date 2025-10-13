using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.InputSystem.Controls;

public class InputManager : MonoBehaviour
{
    private PlayerInput input;
    private PlayerInput.PlayerActionActions playerAction;

    // 마우스 왼쪽 클릭(선택) 이벤트
    public delegate void SelectActionHandler(Vector2 position);

    public event SelectActionHandler OnLMBInput;
    public event SelectActionHandler OnLMBReleased;
    // 마우스 오른쪽 클릭(명령) 이벤트
    public event SelectActionHandler OnRMBInput;

    // 누를때와 땔때를 감지할 때
    public delegate void ButtonPushPullHandler(bool isPressed);

    public event ButtonPushPullHandler OnShiftKeyChanged;
    public event ButtonPushPullHandler OnAKeyChanged;
    public event ButtonPushPullHandler OnMKeyChanged;
    public event ButtonPushPullHandler OnPKeyChanged;

    //인덱스 입력
    public delegate void ButtonOlnyPushHandler(int index);

    public event ButtonOlnyPushHandler SelectGroupNumInput;
    public event ButtonOlnyPushHandler AddGroupInput;
    public event ButtonOlnyPushHandler SkillInput;
    //마우스 스크롤
    public delegate void ScrollHandler(float scrollValue);

    public event ScrollHandler OnScrollInput;

    private void Awake()
    {
        input = new PlayerInput();
        playerAction = input.PlayerAction;

        // Input System의 콜백을 Unity Event로 변환하여 외부에 알립니다.
        playerAction.LeftMouseButton.performed += ctx => OnLMBInput?.Invoke(Mouse.current.position.ReadValue());
        playerAction.LeftMouseButton.canceled += ctx => OnLMBReleased?.Invoke(Mouse.current.position.ReadValue());

        playerAction.RightMouseButton.performed += ctx => OnRMBInput?.Invoke(Mouse.current.position.ReadValue());

        playerAction.LeftShift.performed += ctx => OnShiftKeyChanged?.Invoke(true);
        playerAction.LeftShift.canceled += ctx => OnShiftKeyChanged?.Invoke(false);

        playerAction.Zoom.performed += ctx => OnScrollInput?.Invoke(ctx.ReadValue<float>());

        playerAction.AttackGround.performed += ctx => OnAKeyChanged.Invoke(true);
        playerAction.M.performed += ctx => OnMKeyChanged.Invoke(true);
        playerAction.Patrol.performed += ctx => OnPKeyChanged.Invoke(true);

        playerAction.SelectNumberPad.performed += ctx =>
        {
            var control = ctx.control;
            if (control.displayName == "1")
                SelectGroupNumInput?.Invoke(0);
            else if (control.displayName == "2")
                SelectGroupNumInput?.Invoke(1);
            else if (control.displayName == "3")
                SelectGroupNumInput?.Invoke(2);
            else if (control.displayName == "4")
                SelectGroupNumInput?.Invoke(3);
        };

        playerAction.AddGroup.performed += ctx =>
        {
            if (ctx.control is KeyControl key)
            {
                switch (key.keyCode)
                {
                    case Key.Digit1: AddGroupInput?.Invoke(0); break;
                    case Key.Digit2: AddGroupInput?.Invoke(1); break;
                    case Key.Digit3: AddGroupInput?.Invoke(2); break;
                    case Key.Digit4: AddGroupInput?.Invoke(3); break;
                }
            }
        };

        playerAction.Skill.performed += ctx =>
        {
            if (ctx.control is KeyControl key)
            {
                switch (key.keyCode)
                {
                    case Key.Q: SkillInput?.Invoke(0); break;
                    case Key.W: SkillInput?.Invoke(1); break;
                    case Key.E: SkillInput?.Invoke(2); break;
                    case Key.R: SkillInput?.Invoke(3); break;
                }
            }
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