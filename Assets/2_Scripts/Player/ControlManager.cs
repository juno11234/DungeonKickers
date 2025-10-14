using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ControlManager : MonoBehaviour
{
    private enum KeyType
    {
        None,
        A,
        M,
        P,
        TargetingSkill
    }

    [SerializeField] private InputManager _inputManager;
    [SerializeField] private PlayerSelection _playerSelection;
    [SerializeField] private CameraMovement cameraControl;

    [SerializeField] private Texture2D Acursor;
    [SerializeField] private Texture2D Mcursor;
    [SerializeField] private Texture2D Pcursor;

    [SerializeField] private RectTransform selectionBox;
    [SerializeField] private GameObject skillRadius;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask detectorLayer;

    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;

    private Camera _mainCam;
    private bool _isShiftPressed;
    private bool _isDragging = false;
    private bool _isWide = false;
    private Vector2 _startMousePos;
    private KeyType _keyType;


    void Start()
    {
        _mainCam = Camera.main;

        _inputManager.OnLMBInput += LMBInput;
        _inputManager.OnLMBReleased += LMBReleased;
        _inputManager.OnRMBInput += RMBInput;
        _inputManager.OnScrollInput += cameraControl.Zoom;

        _inputManager.OnShiftKeyChanged += ShiftStatusInput;
        _inputManager.OnAKeyChanged += isPressed => KeyTypeInput(isPressed, KeyType.A);
        _inputManager.OnMKeyChanged += isPressed => KeyTypeInput(isPressed, KeyType.M);
        _inputManager.OnPKeyChanged += isPressed => KeyTypeInput(isPressed, KeyType.P);

        _inputManager.AddGroupInput += AddIndexInput;
        _inputManager.SelectGroupNumInput += SelectIndexInput;
        _inputManager.SkillInput += ReadySkillInput;

        selectionBox.gameObject.SetActive(false);
        _playerSelection.targetSkillChageEvent += TargetSkillReady;
        skillRadius.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (_isDragging)
        {
            Vector2 currentMousePos = Mouse.current.position.ReadValue();
            UpdateSelectionBox(_startMousePos, currentMousePos);
        }
        if (_isWide)
        {
            Vector3 mousePosition = Mouse.current.position.ReadValue();
            Ray ray = _mainCam.ScreenPointToRay(mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 50f, groundLayer))
            {
                Vector3 pos = new Vector3(hit.point.x, 1f, hit.point.z);

                skillRadius.transform.position = pos;
            }
        }
    }

    //쉬프트 누름
    private void ShiftStatusInput(bool isPressed)
    {
        _isShiftPressed = isPressed;
    }
    //입력된 키타입
    private void KeyTypeInput(bool isPressed, KeyType key)
    {
        if (isPressed)
        {
            _keyType = key;
            Texture2D cursorTexture = null;
            switch (_keyType)
            {
                case KeyType.A:
                    cursorTexture = Acursor;
                    break;
                case KeyType.M:
                    cursorTexture = Mcursor;
                    break;
                case KeyType.P:
                    cursorTexture = Pcursor;
                    break;
                case KeyType.TargetingSkill:
                    cursorTexture = currentTargetCursor;
                    break;
            }
            if (cursorTexture != null)
            {
                Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
            }
        }
    }
    Texture2D currentTargetCursor;
    //커서 변경시
    private void TargetSkillReady(Texture2D Cursor, float skillDis, bool wide)
    {
        currentTargetCursor = Cursor;

        if (wide)
        {
            _isWide = wide;
            skillRadius.transform.localScale = new Vector3(skillDis, skillDis, 0f);
            skillRadius.gameObject.SetActive(true);
        }

        KeyTypeInput(true, KeyType.TargetingSkill);
    }

    //입력된 키 취소
    private void ResetControlMode(bool lmb)
    {
        if (_keyType == KeyType.TargetingSkill)
        {
            _isWide = false;
            _playerSelection.CancelTargeting(lmb);
            skillRadius.gameObject.SetActive(false);
        }
        _keyType = KeyType.None;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }


    //번호를 누를때 부대지정
    private void AddIndexInput(int index)
    {
        _playerSelection.AddUnitDesignation(index);
    }
    //부대지정된 유닛들 선택
    private void SelectIndexInput(int index)
    {
        if (Keyboard.current.ctrlKey.isPressed)
            return;
        _playerSelection.SelectUnitDesignation(index);
    }
    private void ReadySkillInput(int index)
    {
        _playerSelection.SelectUnitSkillCheck(index);
    }
    //마우스를 좌클릭 누를때
    private void LMBInput(Vector2 mousePos)
    {
        if (IsPointerOverUI(mousePos))
        {
            return; // UI 클릭이면 게임 입력 무시
        }

        switch (_keyType)
        {
            case KeyType.P:
                HandleGroundOrEnemyInput(mousePos, KeyType.P);
                ResetControlMode(true);
                return;
            case KeyType.A:
                HandleGroundOrEnemyInput(mousePos, KeyType.A);
                ResetControlMode(true);
                return;
            case KeyType.M:
                // Move 모드일 때는 우클릭 로직과 동일
                HandleGroundOrEnemyInput(mousePos, KeyType.M);
                ResetControlMode(true);
                return;
            case KeyType.TargetingSkill:
                HandleTargetSkill(mousePos);
                ResetControlMode(true);
                return;
        }

        _isDragging = true;
        _startMousePos = mousePos;

        // Shift 키가 눌려있지 않으면 드래그 시작 시 기존 선택 해제
        if (_isShiftPressed == false)
        {
            _playerSelection.DeselectAllUnit();
        }
    }
    //마우스를 뗄때 유닛 선택방식결정
    private void LMBReleased(Vector2 endMousePos)
    {
        // 모드가 활성화된 상태라면 드래그 선택 로직 무시
        if (_keyType != KeyType.None)
        {
            return;
        }

        _isDragging = false;
        selectionBox.gameObject.SetActive(false);

        if (Vector2.Distance(_startMousePos, endMousePos) > 10f)
        {
            SelectUnitsInDragBox(_startMousePos, endMousePos);
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
                _playerSelection.SelectUnit(playerUnit, _isShiftPressed);
            }
            else
            {
                if (_isShiftPressed == false)
                {
                    _playerSelection.DeselectAllUnit();
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
                    _playerSelection.SelectUnit(playerUnit, true);
                }
            }
        }
    }
    //우클릭 로직
    private void RMBInput(Vector2 mousePos)
    {
        ResetControlMode(false);

        HandleGroundOrEnemyInput(mousePos, KeyType.M);
    }

    // 레이캐스트를 통해 땅 또는 적을 선택하는 공통 로직
    private void HandleGroundOrEnemyInput(Vector2 mousePos, KeyType keyType)
    {
        Ray ray = _mainCam.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out RaycastHit hit, 50f, ~detectorLayer))
        {
            if (hit.transform.CompareTag("Enemy"))
            {
                var monster = CombatSystem.Instance.GetMonsterOrNull(hit.collider);
                if (monster != null)
                {
                    _playerSelection.SelectUnit_Attack(monster);
                }
            }
            else if (hit.transform.CompareTag("Ground"))
            {
                switch (keyType)
                {
                    case KeyType.None:
                        break;
                    case KeyType.A:
                        _playerSelection.SelectUnit_AttackGround(hit.point);
                        break;
                    case KeyType.M:
                        _playerSelection.SelectUnit_Move(hit.point);
                        break;
                    case KeyType.P:
                        _playerSelection.SelectUnit_Patrol(hit.point);
                        break;
                }
            }
        }
    }
    //스킬 확정시
    private void HandleTargetSkill(Vector2 mousePos)
    {
        Ray ray = _mainCam.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out RaycastHit hit, 50f, ~detectorLayer))
        {
            _playerSelection.TargetingSkillUse(hit.point, hit);
        }
    }

    //마우스 UI위인지 체크
    private bool IsPointerOverUI(Vector2 mousePos)
    {
        if (EventSystem.current == null) return false;

        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = mousePos
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        return results.Count > 0;
    }

}