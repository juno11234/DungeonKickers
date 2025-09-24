using System.Collections.Generic;
using UnityEngine;

public class PlayerSelection : MonoBehaviour
{
    private List<PlayerUnit> _selectedPlayers = new List<PlayerUnit>();
    private Dictionary<int, List<PlayerUnit>> _unitDesignations = new Dictionary<int, List<PlayerUnit>>();

    [Header("Move Settings")]
    public float spreadRadius = 1f;

    // 유닛 선택
    public void SelectUnit(PlayerUnit playerUnit, bool isShiftPressed)
    {
        if (!isShiftPressed)
            DeselectAllUnit();

        if (!_selectedPlayers.Contains(playerUnit))
        {
            _selectedPlayers.Add(playerUnit);
            playerUnit.Selected();
        }
    }

    public void DeselectAllUnit()
    {
        foreach (var player in _selectedPlayers)
            player.deSelected();

        _selectedPlayers.Clear();
    }

    // 부대 지정 (Ctrl+숫자)
    public void AddUnitDesignation(int index)
    {
        if (_selectedPlayers.Count == 0) return;
        _unitDesignations[index] = new List<PlayerUnit>(_selectedPlayers);
    }

    // 부대 선택 (숫자)
    public void SelectUnitDesignation(int index)
    {
        if (!_unitDesignations.TryGetValue(index, out var unitGroup)) return;

        DeselectAllUnit();
        foreach (var unit in unitGroup)
        {
            if (unit == null) continue;
            _selectedPlayers.Add(unit);
            unit.Selected();
        }
    }

    // 유닛 명령 공통 처리
    private void CommandSelectedUnits(Vector3 position, bool useDetector, System.Action<PlayerUnit, Vector3> command)
    {
        if (_selectedPlayers.Count == 0) return;

        if (_selectedPlayers.Count == 1)
        {
            if (useDetector) _selectedPlayers[0].OnDetector();
            else _selectedPlayers[0].OffDetector();

            _selectedPlayers[0].MonsterTargetCancel();
            command(_selectedPlayers[0], position);
            return;
        }

        for (int i = 0; i < _selectedPlayers.Count; i++)
        {
            Vector3 offset = GetDistributedPosition(i, _selectedPlayers.Count, spreadRadius);
            Vector3 destination = position + offset;

            if (useDetector) _selectedPlayers[i].OnDetector();
            else _selectedPlayers[i].OffDetector();

            _selectedPlayers[i].MonsterTargetCancel();
            command(_selectedPlayers[i], destination);
        }
    }

    public void SelectUnit_Move(Vector3 position)
        => CommandSelectedUnits(position, false, (unit, dest) => unit.Move(dest));

    public void SelectUnit_AttackGround(Vector3 position)
        => CommandSelectedUnits(position, true, (unit, dest) => unit.AttackGround(dest));

    public void SelectUnit_Patrol(Vector3 position)
        => CommandSelectedUnits(position, true, (unit, dest) => unit.Patrol(dest));

    public void SelectUnit_Attack(IFighter monster)
    {
        foreach (var player in _selectedPlayers)
            player.AttackTargetSet(monster);
    }

    private Vector3 GetDistributedPosition(int index, int count, float radius)
    {
        float angle = (360f / count) * index * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(angle) * radius, 0, Mathf.Cos(angle) * radius);
    }
}
