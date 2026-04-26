using UnityEngine;

[CreateAssetMenu(fileName = "Trait", menuName = "Data/TraitSkill")]
public class TraitSkill : Skill
{
    [Header("특성 스킬 설정")]
    public int requiredTier; // 이 스킬이 속한 티어
    public int requiredPointsInTier; // 이 티어에 투자해야 하는 최소 포인트

    public override void ApplyEffect(PlayerUnit unit, int currentLevel)
    {

    }
}
