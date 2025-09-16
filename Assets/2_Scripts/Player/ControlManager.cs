using UnityEngine;

public class ControlManager : MonoBehaviour
{
    [SerializeField] private InputManager _inputManager;
    [SerializeField] private PlayerSelection _playerSelection;
    private bool _isShiftPressed;

    private Camera _mainCam;

    void Start()
    {
        _mainCam = Camera.main;
        _inputManager.OnSelectAction += HandleSelectInput;
        _inputManager.OnMoveOrAttackAction += HandleMoveOrAttackInput;
        _inputManager.OnShiftStatusChanged += UpdateShiftStatus;
    }

    private void UpdateShiftStatus(bool isPressed)
    {
        _isShiftPressed = isPressed;
    }

    private void HandleSelectInput(Vector2 mousePosition)
    {
        Ray ray = _mainCam.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.TryGetComponent(out PlayerUnit playerUnit))
            {
                _playerSelection.SelectPlayer(playerUnit, _isShiftPressed);
            }
            else
            {
                // Shift 키가 눌려있지 않으면 전체 선택 해제
                if (_isShiftPressed==false)
                {
                    _playerSelection.DeselectAllPlayers();
                }
            }
        }
    }


    private void HandleMoveOrAttackInput(Vector2 mousePosition)
    {
        Ray ray = _mainCam.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.CompareTag("Enemy"))
            {
                var monster = CombatSystem.Instance.GetMonsterOrNull(hit.collider);
                if (monster != null)
                {
                    _playerSelection.AttackSelectedPlayers(monster);
                }
            }
            else if (hit.transform.CompareTag("Ground"))
            {
                _playerSelection.MoveSelectedPlayers(hit.point);                
            }
        }
    }
}