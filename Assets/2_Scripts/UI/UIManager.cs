using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private Dictionary<int, GameObject> _skillUIIcons = new Dictionary<int, GameObject>();
    private Dictionary<PlayerUnit, UnitStats> _playerStats = new Dictionary<PlayerUnit, UnitStats>();

    [SerializeField] private GameObject[] IconUI;

    [SerializeField] private GameObject infoBar;
    [SerializeField] private TMP_Text AtkUI;
    [SerializeField] private TMP_Text DefUI;
    [SerializeField] private TMP_Text JobNameUI;
    [SerializeField] private TMP_Text HpUI;
    [SerializeField] private Image portrait;
    // UI 초기화
    public void InitializeUI(PlayerUnit[] allPlayers)
    {
        for (int i = 0; i < allPlayers.Length; i++)
        {
            GameObject uiIcon = Instantiate(allPlayers[i].GetSkillIconUI(), IconUI[i].transform);
            _skillUIIcons.Add(allPlayers[i].GetSkillID(), uiIcon);

            SkillTimerUI skilltimer = uiIcon.GetComponent<SkillTimerUI>();
            skilltimer.Init(allPlayers[i]);

            _playerStats.Add(allPlayers[i], allPlayers[i].Stats);

            uiIcon.SetActive(false);
            allPlayers[i].HpChanged += HpUpdate;
            allPlayers[i].StatChanged += StatUpdate;
        }
        infoBar.SetActive(false);

    }

    // 유닛 선택 시 아이콘 활성화
    public void ActivateSkillIcons(HashSet<PlayerUnit> selectedUnits)
    {
        foreach (var unit in selectedUnits)
        {
            int id = unit.GetSkillID();
            if (_skillUIIcons.TryGetValue(id, out var icon))
            {
                icon.SetActive(true);
            }
            if (selectedUnits.Count == 1)
            {
                infoBar.SetActive(true);
                _playerStats.TryGetValue(unit, out UnitStats stat);
                StatUpdate(stat.damage, stat.guard);
                Sprite port = unit.GetPortrait();
                portrait.sprite = port;
                JobNameUI.text = stat.name;
                HpUpdate(unit.CurrentHp, unit.MaxHp);

            }
        }

    }

    private void StatUpdate(int damage, int guard)
    {
        AtkUI.text = damage.ToString();
        DefUI.text = guard.ToString();
    }

    private void HpUpdate(int current, int max)
    {
        string hp = $"{current} / {max}";
        HpUI.text = hp;
    }

    // 유닛 선택 해제 시 아이콘 비활성화
    public void DeactivateSkillIcons(IEnumerable<PlayerUnit> selectedUnits)
    {
        foreach (var unit in selectedUnits)
        {
            int id = unit.GetSkillID();
            if (_skillUIIcons.TryGetValue(id, out var icon))
            {
                icon.SetActive(false);
            }
        }
    }
}
