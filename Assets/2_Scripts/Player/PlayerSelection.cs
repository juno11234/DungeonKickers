using System;
using System.Collections.Generic;
using System.Linq; // LINQ를 사용하여 리스트에서 null을 쉽게 제거하기 위해 추가
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelection : MonoBehaviour
{
    [SerializeField] private PlayerUnit[] allPlayer;
   
    [SerializeField] private UIManager uiManager;

    // List 대신 HashSet을 사용하여 중복을 방지하고 추가/제거/검색 성능을 향상
    private HashSet<PlayerUnit> _selectedPlayers = new HashSet<PlayerUnit>();
    private Dictionary<int, List<PlayerUnit>> _unitDesignations = new Dictionary<int, List<PlayerUnit>>();

    TargetingSkillerBase currentTargetingSkiller;
    public event Action<Texture2D, float, bool> targetSkillChageEvent;

    [Header("Move Settings")]
    public float spreadRadius = 1f;

    private void Start()
    {
        for (int i = 0; i < allPlayer.Length; i++)
        {
            if (allPlayer[i] is TargetingSkillerBase targetingSkiller)
            {
                targetingSkiller.cursorChangeEvent += TargetingCursorChange;
            }
            
        }
        uiManager.InitializeUI(allPlayer);
    }
  
    // 유닛 선택
    public void SelectUnit(PlayerUnit playerUnit, bool isShiftPressed)
    {
        if (isShiftPressed == false)
        {
            DeselectAllUnit();
        }

        // HashSet의 Add 메서드는 중복된 항목이 없을 때만 true를 반환
        if (_selectedPlayers.Add(playerUnit))
        {
            playerUnit.Selected();
        }

        uiManager.ActivateSkillIcons(_selectedPlayers);
    }

    public void DeselectAllUnit()
    {
        foreach (var player in _selectedPlayers)
        {
            if (player != null) // 유닛이 파괴되었을 경우를 대비
            {
                
                player.deSelected();              
            }
        }
        uiManager.DeactivateSkillIcons(_selectedPlayers);
        _selectedPlayers.Clear();
    }

    // 부대 지정 (Ctrl+숫자)
    public void AddUnitDesignation(int index)
    {
        if (_selectedPlayers.Count == 0) return;

        // 지정하기 전에 현재 선택된 유닛 중 null이 아닌 유닛만 리스트에 추가
        _unitDesignations[index] = _selectedPlayers.Where(unit => unit != null).ToList();
    }

    // 부대 선택 (숫자)
    public void SelectUnitDesignation(int index)
    {
        if (_unitDesignations.TryGetValue(index, out var unitGroup) == false) return;

        // 부대 목록에서 파괴된(null) 유닛을 제거
        unitGroup.RemoveAll(item => item == null);

        DeselectAllUnit();
        foreach (var unit in unitGroup)
        {
            if (_selectedPlayers.Add(unit))
            {
                unit.Selected();
            }
        }
        uiManager.ActivateSkillIcons(_selectedPlayers);
    }
    //스킬사용이가능한가

    public void SelectUnitSkillCheck(int index)
    {
        foreach (var player in _selectedPlayers)
        {
            if (player.SkillCheck(index))
            {
                Debug.Log("스킬" + index);
                if (player is TargetingSkillerBase targetSkiller)
                {
                    currentTargetingSkiller = targetSkiller;
                    currentTargetingSkiller.CursorChange();
                }
                else
                {
                    player.Skill();
                }
            }
        }
    }

    //커서만 바꿈
    public void TargetingCursorChange(Texture2D skillCursor, float skillDis, bool wide)
    {
        //어떻게 스킬범위에맞게 마우스 커서를 만들어주지?
        //레이를 업데이트로 쏘게 만들어서 계속 따라다니게 만든다
        targetSkillChageEvent?.Invoke(skillCursor, skillDis, wide);
    }
    //스킬발동 확정
    public void TargetingSkillUse(Vector3 pos, RaycastHit rayHit)
    {
        //어떻게 파이어볼에게 pos를 전달해주지?

        if (currentTargetingSkiller is Player_Priest priest)
        {
            if (rayHit.transform.gameObject.TryGetComponent(out PlayerUnit unit))
            {
                priest.StartFollowAndCast(unit);
            }
            return;
        }
        currentTargetingSkiller.TargetPos = pos;
        currentTargetingSkiller.RangeCheck();
    }

    public void CancelTargeting(bool lmb)
    {
        currentTargetingSkiller.TargetCancel(lmb);
    }
    // 유닛 명령 공통 처리
    private void CommandSelectedUnits(Vector3 position, bool useDetector, Action<PlayerUnit, Vector3> command)
    {
        // 명령을 내리기 전, 파괴된 유닛이 있다면 목록에서 제거
        _selectedPlayers.RemoveWhere(unit => unit == null);

        if (_selectedPlayers.Count == 0) return;

        // LINQ를 사용하여 살아있는 유닛 리스트를 만듦 (명령 처리를 위해)
        List<PlayerUnit> aliveUnits = _selectedPlayers.ToList();

        if (aliveUnits.Count == 1)
        {
            PlayerUnit unit = aliveUnits[0];
            if (useDetector) unit.OnDetector();
            else unit.OffDetector();

            unit.MonsterTargetCancel();
            command(unit, position);
            return;
        }

        for (int i = 0; i < aliveUnits.Count; i++)
        {
            Vector3 offset = GetDistributedPosition(i, aliveUnits.Count, spreadRadius);
            Vector3 destination = position + offset;

            PlayerUnit unit = aliveUnits[i];

            if (useDetector) unit.OnDetector();
            else unit.OffDetector();

            unit.MonsterTargetCancel();
            command(unit, destination);
        }
    }

    // 람다식을 사용한 간결한 메서드 호출 
    public void SelectUnit_Move(Vector3 position)
        => CommandSelectedUnits(position, false, (unit, dest) => unit.Move(dest));

    public void SelectUnit_AttackGround(Vector3 position)
        => CommandSelectedUnits(position, true, (unit, dest) => unit.AttackGround(dest));

    public void SelectUnit_Patrol(Vector3 position)
        => CommandSelectedUnits(position, true, (unit, dest) => unit.Patrol(dest));

    public void SelectUnit_Attack(IFighter monster)
    {
        // 공격 명령 시에도 파괴된 유닛은 제외
        foreach (var player in _selectedPlayers)
        {
            if (player != null)
                player.AttackTargetSet(monster);
        }
    }

    private Vector3 GetDistributedPosition(int index, int count, float radius)
    {
        float angle = (360f / count) * index * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(angle) * radius, 0, Mathf.Cos(angle) * radius);
    }
}