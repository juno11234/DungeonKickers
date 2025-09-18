using UnityEngine;
using UnityEngine.InputSystem;

public class ControlManager : MonoBehaviour
{
    [SerializeField] private InputManager _inputManager;
    [SerializeField] private PlayerSelection _playerSelection;
    [SerializeField] private RectTransform selectionBox;


    private Camera _mainCam;
    private bool _isShiftPressed;
    private bool isDragging = false;
    private Vector2 startMousePos;

    void Start()
    {
        _mainCam = Camera.main;
        _inputManager.OnSelectAction += HandleSelectInput;
        _inputManager.OnMoveOrAttackAction += HandleMoveOrAttackInput;
        _inputManager.OnShiftStatusChanged += UpdateShiftStatus;

        _inputManager.OnSelectReleased += HandleSelectReleased;

        selectionBox.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (isDragging)
        {
            Vector2 currentMousePos = Mouse.current.position.ReadValue();
            UpdateSelectionBox(startMousePos, currentMousePos);
        }
    }

    private void UpdateShiftStatus(bool isPressed)
    {
        _isShiftPressed = isPressed;
    }

    //마우스를 누를때
    private void HandleSelectInput(Vector2 mousePos)
    {
        isDragging = true;
        startMousePos = mousePos;

        // Shift 키가 눌려있지 않으면 드래그 시작 시 기존 선택 해제
        if (_isShiftPressed == false)
        {
            _playerSelection.DeselectAllPlayers();
        }
    }

    //마우스를 뗄때 유닛 선택방식결정
    private void HandleSelectReleased(Vector2 endMousePos)
    {
        isDragging = false;
        selectionBox.gameObject.SetActive(false);

        if (Vector2.Distance(startMousePos, endMousePos) > 10f)
        {
            SelectUnitsInDragBox(startMousePos, endMousePos);
        }
        else
        {
            SelectSingleUnit(endMousePos);
        }
    }

    // 마우스 단일 클릭 유닛 선택 로직
    private void SelectSingleUnit(Vector2 mousePos)
    {
        Ray ray = _mainCam.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.TryGetComponent(out PlayerUnit playerUnit))
            {
                _playerSelection.SelectPlayer(playerUnit, _isShiftPressed);
            }
            else
            {
                if (_isShiftPressed == false)
                {
                    _playerSelection.DeselectAllPlayers();
                }
            }
        }
    }

    //UI에 맞춰 오버랩박스 생성
    private void SelectUnitsInDragBox(Vector2 startPos, Vector2 endPos)
    {
        Vector3 min = _mainCam.ScreenToWorldPoint(new Vector3(Mathf.Min(startPos.x, endPos.x), Mathf.Min(startPos.y, endPos.y), _mainCam.nearClipPlane));
        Vector3 max = _mainCam.ScreenToWorldPoint(new Vector3(Mathf.Max(startPos.x, endPos.x), Mathf.Max(startPos.y, endPos.y), _mainCam.nearClipPlane));

        // OverlapBox의 중앙점과 크기 계산
        Vector3 center = (min + max) / 2f;
        Vector3 size = max - min;
        size.z = 100f; // z축으로 충분한 깊이를 줌

        Collider[] hitColliders = Physics.OverlapBox(center, size / 2, Quaternion.identity);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.TryGetComponent(out PlayerUnit playerUnit))
            {
                _playerSelection.SelectPlayer(playerUnit, true); // 드래그 선택은 항상 Shift키 누른 것처럼 작동
            }
        }
    }

    // 드래그 박스 UI 업데이트
    private void UpdateSelectionBox(Vector2 startPos, Vector2 currentPos)
    {
        selectionBox.gameObject.SetActive(true);

        Vector2 min = Vector2.Min(startPos, currentPos);
        Vector2 max = Vector2.Max(startPos, currentPos);

        selectionBox.anchoredPosition = min;
        selectionBox.sizeDelta = max - min;
    }

    //우클릭 로직
    private void HandleMoveOrAttackInput(Vector2 mousePos)
    {
        Ray ray = _mainCam.ScreenPointToRay(mousePos);
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