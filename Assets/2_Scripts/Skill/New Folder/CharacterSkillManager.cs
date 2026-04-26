using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
public enum BuffType
{
    None,
    Hp,
    Def,
    Atk,
    Ats,
    Range,
    Speed
}
public enum SkillOwner
{
    None,
    Warrior,
    Rogue,
    Mage,
    Priest
}

public class CharacterSkillManager : MonoBehaviour
{
    [Header("플레이어 목록")]
    [SerializeField] private List<PlayerUnit> playerUnits;

    [Header("직업별 패시브 스킬들")]
    [SerializeField] private List<PassiveSkill> warriorPassives;
    [SerializeField] private List<PassiveSkill> thiefPassives;
    [SerializeField] private List<PassiveSkill> magePassives;
    [SerializeField] private List<PassiveSkill> priestPassives;

    private Dictionary<PlayerUnit, List<PassiveSkill>> passiveDict;
    
    private void Awake()
    {
        passiveDict = new Dictionary<PlayerUnit, List<PassiveSkill>>
        {
            { playerUnits[0], warriorPassives },
            { playerUnits[1], thiefPassives },
            { playerUnits[2], magePassives },
            { playerUnits[3], priestPassives }
        };
    }

    public void ApplyPassives(PlayerUnit unit, int tier, bool left)
    {
        if (passiveDict.ContainsKey(unit) == false)
        {
            return;
        }

        List<PassiveSkill> skills = passiveDict[unit];
        int index;

        switch (tier)
        {
            case 1:
                index = 0;
                break;
            case 2:
                index = 2;
                break;
            default:
                index = 0;
                break;
        }

        if (skills[index].currentLevel + skills[index + 1].currentLevel < 5)
        {
            int level = skills[index].currentLevel++;
            skills[index].ApplyEffect(unit, level);
        }

    }
}

