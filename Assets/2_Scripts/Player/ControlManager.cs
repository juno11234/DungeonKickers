using System;
using System.Drawing;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ControlManager : MonoBehaviour
{

    [SerializeField] private InputManager _inputManager;
    [SerializeField] private PlayerSelection _playerSelection;
    [SerializeField] private CameraMovement cameraControl;

    [SerializeField] private Texture2D Acursor;
    [SerializeField] private Texture2D Mcursor;
    [SerializeField] private RectTransform selectionBox;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask detectorLayer;

    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;
    private Camera _mainCam;
    private bool _isShiftPressed;
    private bool _isAKeyPressed;
    private bool _isMkeyPressed;
    private bool isDragging = false;
    private Vector2 startMousePos;

    void Start()
    {
        _mainCam = Camera.main;
        _inputManager.OnSelectAction += HandleSelectInput;
        _inputManager.OnMoveOrAttackAction += HandleMoveOrAttackInput;
        _inputManager.OnShiftStatusChanged += UpdateShiftStatus;
        _inputManager.OnAKeyChanged += UpdateAKeyStatus;
        _inputManager.OnMKeyChanged += UpdateMKeyStatus;
        _inputManager.OnSelectReleased += HandleSelectReleased;

        _inputManager.OnScrollInput += cameraControl.Zoom;

        _inputManager.AddGroup += AddIndex;
        _inputManager.SelectGroup += SelectIndex;


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

    //쉬프트 누름
    private void UpdateShiftStatus(bool isPressed)
    {
        _isShiftPressed = isPressed;
    }
    public void UpdateAKeyStatus(bool isPressed)
    {
        _isAKeyPressed = isPressed;
        _isMkeyPressed = false;
        Cursor.SetCursor(Acursor, hotSpot, cursorMode);
    }
    public void UpdateMKeyStatus(bool isPressed)
    {
        Debug.Log("복합");
        _isMkeyPressed = isPressed;
        _isAKeyPressed = false;
        Cursor.SetCursor(Mcursor, hotSpot, cursorMode);
    }

    //번호를 누를때 부대지정
    private void AddIndex(int index)
    {
        
        _playerSelection.AddUnitDesignations(index);
    }
    private void SelectIndex(int index)
    {
        if (Keyboard.current.ctrlKey.isPressed)
            return;
        _playerSelection.SelectUnitDesignations(index);
    }

    //마우스를 좌클릭 누를때
    private void HandleSelectInput(Vector2 mousePos)
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;


        if (_isAKeyPressed)
        {
            AttackGround(mousePos);
            return;
        }
        else if (_isMkeyPressed)
        {

            HandleMoveOrAttackInput(mousePos);
            return;
        }

        isDragging = true;
        startMousePos = mousePos;

        // Shift 키가 눌려있지 않으면 드래그 시작 시 기존 선택 해제
        if (_isShiftPressed == false)
        {
            _playerSelection.DeselectAllPlayers();
        }
    }
    private void AttackGround(Vector2 mousePos)
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        Ray ray = _mainCam.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 50f, ~detectorLayer))
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
                _playerSelection.MoveAndAttackForAttackGround(hit.point);
            }
        }
    }

    //마우스를 뗄때 유닛 선택방식결정
    private void HandleSelectReleased(Vector2 endMousePos)
    {
        if (_isAKeyPressed)
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            _isAKeyPressed = false;
            return;
        }

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

        if (Physics.Raycast(ray, out hit, 50f, ~detectorLayer))
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
        Ray startRay = _mainCam.ScreenPointToRay(startPos);
        Ray endRay = _mainCam.ScreenPointToRay(endPos);

        if (Physics.Raycast(startRay, out RaycastHit startHit, Mathf.Infinity, groundLayer) &&
            Physics.Raycast(endRay, out RaycastHit endHit, Mathf.Infinity, groundLayer))
        {
            Vector3 dragMinWorld = startHit.point;
            Vector3 dragMaxWorld = endHit.point;

            float boxY = (dragMinWorld.y + dragMaxWorld.y) / 2f;
            dragMinWorld.y = boxY;
            dragMaxWorld.y = boxY;

            Vector3 center = (dragMinWorld + dragMaxWorld) / 2f;
            Vector3 size = dragMaxWorld - dragMinWorld;

            // 크기가 항상 양수가 되도록 Mathf.Abs() 사용
            size.x = Mathf.Abs(size.x);
            size.y = 10f;
            size.z = Mathf.Abs(size.z);

            Collider[] hitColliders = Physics.OverlapBox(center, size / 2, Quaternion.identity);

            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.TryGetComponent(out PlayerUnit playerUnit))
                {
                    _playerSelection.SelectPlayer(playerUnit, true);
                }
            }
        }
    }

    // 드래그 박스 UI 업데이트
    private void UpdateSelectionBox(Vector2 startPos, Vector2 currentPos)
    {
        selectionBox.gameObject.SetActive(true);

        Vector2 min = Vector2.Min(startPos, currentPos);
        Vector2 max = Vector2.Max(startPos, currentPos);
        //앵커 피벗을 설정
        selectionBox.anchoredPosition = min;
        //사이즈를 설정
        selectionBox.sizeDelta = max - min;
    }

    //우클릭 로직
    private void HandleMoveOrAttackInput(Vector2 mousePos)
    {
        if (_isAKeyPressed || _isMkeyPressed)
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            _isAKeyPressed = false;
            _isMkeyPressed = false;
        }

        Ray ray = _mainCam.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 50f, ~detectorLayer))
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