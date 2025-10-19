using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private Dictionary<int, GameObject> _skillUIIcons = new Dictionary<int, GameObject>();
    [SerializeField] private GameObject[] IconUI;
    // UI 초기화
    public void InitializeUI(PlayerUnit[] allPlayers)
    {
        for (int i = 0; i < allPlayers.Length; i++)
        {
            GameObject uiIcon = Instantiate(allPlayers[i].GetSkillIconUI(), IconUI[i].transform);
            _skillUIIcons.Add(allPlayers[i].GetSkillID(), uiIcon);
            SkillTimerUI skilltimer = uiIcon.GetComponent<SkillTimerUI>();
            skilltimer.Init(allPlayers[i]);
            uiIcon.SetActive(false);
        }
    }

    // 유닛 선택 시 아이콘 활성화
    public void ActivateSkillIcons(IEnumerable<PlayerUnit> selectedUnits)
    {
        foreach (var unit in selectedUnits)
        {
            int id = unit.GetSkillID();
            if (_skillUIIcons.TryGetValue(id, out var icon))
                icon.SetActive(true);
        }
    }

    // 유닛 선택 해제 시 아이콘 비활성화
    public void DeactivateSkillIcons(IEnumerable<PlayerUnit> selectedUnits)
    {
        foreach (var unit in selectedUnits)
        {
            int id = unit.GetSkillID();
            if (_skillUIIcons.TryGetValue(id, out var icon))
                icon.SetActive(false);
        }
    }
}
