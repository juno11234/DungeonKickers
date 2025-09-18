using System.Collections.Generic;
using UnityEngine;

public class PlayerSelection : MonoBehaviour
{
    private List<PlayerUnit> _selectedPlayers = new List<PlayerUnit>();
    public float spreadRadius = 1f;
    public void SelectPlayer(PlayerUnit playerUnit, bool isShiftPressed)
    {
        if (isShiftPressed == false)
        {
            DeselectAllPlayers();
        }

        if (_selectedPlayers.Contains(playerUnit) == false)
        {
            _selectedPlayers.Add(playerUnit);
            playerUnit.Selected();
        }

        // Debug.Log($"선택된 유닛 수: {_selectedPlayers.Count}");
    }

    public void DeselectAllPlayers()
    {
        foreach (PlayerUnit player in _selectedPlayers)
        {
            player.CanceledSelected();
        }
        _selectedPlayers.Clear();
    }

    public void MoveSelectedPlayers(Vector3 position)
    {
        // 선택된 유닛이 1명일 때는 분산하지 않고 바로 이동
        if (_selectedPlayers.Count == 1)
        {
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

            _selectedPlayers[i].MonsterTargetCancel();
            _selectedPlayers[i].Move(destination);
        }
    }
    private Vector3 GetDistributedPosition(int index, int count, float radius)
    {
        // 원의 둘레를 따라 각도 계산
        float angle = (360f / count) * index * Mathf.Deg2Rad;

        // 원형 위치 계산
        float x = Mathf.Sin(angle) * radius;
        float z = Mathf.Cos(angle) * radius;

        return new Vector3(x, 0, z);
    }

    public void AttackSelectedPlayers(IFighter monster)
    {
        foreach (PlayerUnit player in _selectedPlayers)
        {
            player.AttackTargetSet(monster);
        }
    }
}