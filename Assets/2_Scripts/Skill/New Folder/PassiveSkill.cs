using UnityEngine;

[CreateAssetMenu(fileName = "Passive", menuName = "Data/PassiveSkill")]
public class PassiveSkill : Skill
{
    [Header("패시브 스킬 설정")]
    public int maxLevel = 5;
    public int currentLevel;
    public BuffType type;

    public override void ApplyEffect(PlayerUnit unit, int currentLevel)
    {
        unit.ApplyPassiveSkill(type, currentLevel);
    }
}
