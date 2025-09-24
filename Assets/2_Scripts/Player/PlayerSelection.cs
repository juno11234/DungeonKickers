using System.Collections.Generic;
using UnityEngine;

public class PlayerSelection : MonoBehaviour
{
    private List<PlayerUnit> _selectedPlayers = new List<PlayerUnit>();
    private Dictionary<int, PlayerUnit[]> _unitDesignations = new Dictionary<int, PlayerUnit[]>();
    PlayerUnit[] group1 = new PlayerUnit[4];
    PlayerUnit[] group2 = new PlayerUnit[4];
    PlayerUnit[] group3 = new PlayerUnit[4];
    PlayerUnit[] group4 = new PlayerUnit[4];

    public float spreadRadius = 1f;
    private void Start()
    {
        _unitDesignations.Add(0, group1);
        _unitDesignations.Add(1, group2);
        _unitDesignations.Add(2, group3);
        _unitDesignations.Add(3, group4);
    }
    //유닛선택 로직
    public void SelectUnit(PlayerUnit playerUnit, bool isShiftPressed)
    {
        if (isShiftPressed == false)
        {
            DeselectAllUnit();
        }

        if (_selectedPlayers.Contains(playerUnit) == false)
        {
            _selectedPlayers.Add(playerUnit);
            playerUnit.Selected();
        }

    }
    //유닛 전체해제 로직
    public void DeselectAllUnit()
    {
        foreach (PlayerUnit player in _selectedPlayers)
        {
            player.deSelected();
        }
        _selectedPlayers.Clear();
    }

    //부대지정 로직
    public void AddUnit_Designations(int index)
    {
        if (_unitDesignations.TryGetValue(index, out PlayerUnit[] unit) && _selectedPlayers.Count > 0)
        {

            for (int i = 0; i < _selectedPlayers.Count; i++)
            {
                unit[i] = _selectedPlayers[i];
            }
        }

    }

    //부대선택 로직
    public void SelectUnit_Designations(int index)
    {
        if (_unitDesignations.TryGetValue(index, out PlayerUnit[] unit))
        {
            DeselectAllUnit();
            foreach (PlayerUnit oneUnit in unit)
            {
                if (oneUnit == null) continue;

                _selectedPlayers.Add(oneUnit);
                oneUnit.Selected();

            }
        }

    }
  
    //지정된 지점으로 Move로직
    public void SelectUnit_Move(Vector3 position)
    {
        // 선택된 유닛이 1명일 때는 분산하지 않고 바로 이동
        if (_selectedPlayers.Count == 1)
        {
            _selectedPlayers[0].OffDetector();
            _selectedPlayers[0].MonsterTargetCancel();
            _selectedPlayers[0].Move(position);
            return;
        }

        // 여러 유닛이 선택되었을 경우 목표 지점 분산
        for (int i = 0; i < _selectedPlayers.Count; i++)
        {
            // 목표 지점을 중심으로 원형으로 퍼질 위치 계산
            Vector3 offset = GetDistributedPosition(i, _selectedPlayers.Count, spreadRadius);
            Vector3 destination = position + offset;

            _selectedPlayers[i].OffDetector();
            _selectedPlayers[i].MonsterTargetCancel();
            _selectedPlayers[i].Move(destination);
        }
    }
        
    //마우스 위치의 적 공격하는 로직
    public void SelectUnit_Attack(IFighter monster)
    {
        foreach (PlayerUnit player in _selectedPlayers)
        {
            player.AttackTargetSet(monster);
        }
    }

    //어택땅시 이동하다 적발견시 공격로직
    public void SelectUnit_AttackGround(Vector3 position)
    {
        if (_selectedPlayers.Count == 1)
        {
            _selectedPlayers[0].OnDetector();
            _selectedPlayers[0].MonsterTargetCancel();
            _selectedPlayers[0].AttackGround(position);

            return;
        }

        for (int i = 0; i < _selectedPlayers.Count; i++)
        {
            Vector3 offset = GetDistributedPosition(i, _selectedPlayers.Count, spreadRadius);
            Vector3 destination = position + offset;

            _selectedPlayers[i].OnDetector();
            _selectedPlayers[i].MonsterTargetCancel();
            _selectedPlayers[i].AttackGround(destination);
        }
    }

    //패트롤 기능 미완성
    public void SelectUnit_Patrol(Vector3 position)
    {
        if (_selectedPlayers.Count == 1)
        {
            _selectedPlayers[0].OnDetector();
            _selectedPlayers[0].MonsterTargetCancel();
           // _selectedPlayers[0].Patrol(position);

            return;
        }

        for (int i = 0; i < _selectedPlayers.Count; i++)
        {
            Vector3 offset = GetDistributedPosition(i, _selectedPlayers.Count, spreadRadius);
            Vector3 destination = position + offset;

            _selectedPlayers[i].OnDetector();
            _selectedPlayers[i].MonsterTargetCancel();
           // _selectedPlayers[i].Patrol(destination);
        }
    }

    //여러유닛 선택시 겹치지않게 계산하는 로직
    private Vector3 GetDistributedPosition(int index, int count, float radius)
    {
        // 원의 둘레를 따라 각도 계산
        float angle = (360f / count) * index * Mathf.Deg2Rad;

        // 원형 위치 계산
        float x = Mathf.Sin(angle) * radius;
        float z = Mathf.Cos(angle) * radius;

        return new Vector3(x, 0, z);
    }
}