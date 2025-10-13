using UnityEngine;

[CreateAssetMenu(fileName = "New Passive Skill", menuName = "Skill System/Passive Skill")]
public class PassiveSkill : Skill
{
    [Header("패시브 스킬 설정")]
    public int maxLevel = 5;

    // 예시: StatType을 enum으로 관리하면 더 좋습니다.
    // public StatType targetStat; 
    public float[] levelUpBonuses; // 각 레벨마다 증가할 스탯 수치 (배열 크기 = maxLevel)

    public override void ApplyEffect(GameObject character)
    {
        // 실제 게임에서는 character.GetComponent<PlayerStats>().AddBonus(targetStat, bonus); 와 같이 구현
        Debug.Log($"{skillName} 효과 적용! (캐릭터: {character.name})");
    }
}
