using System.Collections.Generic;
using UnityEngine;

public class PlayerSelection : MonoBehaviour
{
    private List<PlayerUnit> _selectedPlayers = new List<PlayerUnit>();

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

        Debug.Log($"선택된 유닛 수: {_selectedPlayers.Count}");
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
        foreach (PlayerUnit player in _selectedPlayers)
        {
            player.MonsterTargetCancel();
            player.Move(position);
        }
    }

    public void AttackSelectedPlayers(IFighter monster)
    {
        foreach (PlayerUnit player in _selectedPlayers)
        {
            player.Attack(monster);
        }
    }
}