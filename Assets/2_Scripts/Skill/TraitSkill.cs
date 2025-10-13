using UnityEngine;

[CreateAssetMenu(fileName = "New Trait Skill", menuName = "Skill System/Trait Skill")]
public class TraitSkill : Skill
{
    [Header("특성 스킬 설정")]
    public int requiredTier; // 이 스킬이 속한 티어
    public int requiredPointsInTier; // 이 티어에 투자해야 하는 최소 포인트

    public override void ApplyEffect(GameObject character)
    {
        // 예: character.GetComponent<FireballSkill>().EnableBurnEffect();
        Debug.Log($"특성 스킬 '{skillName}' 활성화! (캐릭터: {character.name})");
    }
}
