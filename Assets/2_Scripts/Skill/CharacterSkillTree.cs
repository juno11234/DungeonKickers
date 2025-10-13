// CharacterSkillTree.cs
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class CharacterSkillTree : MonoBehaviour
{
    // 각 티어의 스킬들을 관리하기 편하도록 클래스로 묶습니다.
    [System.Serializable]
    public class SkillTier
    {
        public List<PassiveSkill> passiveSkills;
        public TraitSkill traitSkill;
    }

    public SkillTier[] tiers; // 1티어, 2티어...

    [Header("캐릭터 정보")]
    public int level = 1;
    public int totalSkillPoints = 1;
    public int usedSkillPoints = 0;
    public int gold = 1000;

    // 어떤 스킬을 몇 레벨까지 배웠는지 저장합니다.
    public Dictionary<Skill, int> learnedSkills = new Dictionary<Skill, int>();

    public UnityEvent OnSkillChanged; // UI 업데이트를 위한 이벤트

    // 스킬 배우기/레벨업 시도
    public void LearnSkill(Skill skill)
    {
        if (skill is PassiveSkill passiveSkill)
        {
            HandlePassiveSkill(passiveSkill);
        }
        else if (skill is TraitSkill traitSkill)
        {
            HandleTraitSkill(traitSkill);
        }
    }

    // 패시브 스킬 처리
    private void HandlePassiveSkill(PassiveSkill skill)
    {
        int currentLevel;
        if (learnedSkills.ContainsKey(skill))
        {
            currentLevel = learnedSkills[skill];
        }
        else
        {
            currentLevel = 0;
        }

        if (currentLevel >= skill.maxLevel)
        {
            Debug.Log("이미 마스터한 스킬입니다.");
            return;
        }

        if (totalSkillPoints - usedSkillPoints < 1)
        {
            Debug.Log("스킬 포인트가 부족합니다.");
            return;
        }

        usedSkillPoints++;
        learnedSkills[skill] = currentLevel + 1;
        Debug.Log($"{skill.skillName} 레벨업! (현재 레벨: {currentLevel + 1})");
        skill.ApplyEffect(this.gameObject); // 캐릭터에게 효과 적용
        OnSkillChanged.Invoke();
    }

    // 특성 스킬 처리
    private void HandleTraitSkill(TraitSkill skill)
    {
        if (learnedSkills.ContainsKey(skill))
        {
            Debug.Log("이미 배운 특성 스킬입니다.");
            return;
        }

        int pointsInTier = GetPointsInTier(skill.requiredTier);
        if (pointsInTier < skill.requiredPointsInTier)
        {
            Debug.Log($"특성 스킬을 배우려면 {skill.requiredTier}티어에 {skill.requiredPointsInTier}포인트가 필요합니다. (현재: {pointsInTier}포인트)");
            return;
        }
        // 특성 스킬은 포인트 소모가 없다고 가정. 만약 소모한다면 아래 코드 추가
        // if (totalSkillPoints - usedSkillPoints < 1) { /* 포인트 부족 처리 */ }
        // usedSkillPoints++;

        learnedSkills[skill] = 1;
        Debug.Log($"특성 스킬 '{skill.skillName}' 습득!");
        skill.ApplyEffect(this.gameObject); // 캐릭터에게 효과 적용
        OnSkillChanged.Invoke();
    }

    // 특정 티어에 투자된 포인트 계산
    public int GetPointsInTier(int tierIndex)
    {
        if (tierIndex < 0 || tierIndex >= tiers.Length) return 0;

        int points = 0;
        foreach (var pSkill in tiers[tierIndex].passiveSkills)
        {
            if (learnedSkills.ContainsKey(pSkill))
            {
                points += learnedSkills[pSkill];
            }
        }
        return points;
    }

    // 스킬 초기화
    public void Respec(int cost)
    {
        if (gold < cost)
        {
            Debug.Log("초기화에 필요한 골드가 부족합니다.");
            return;
        }

        gold -= cost;
        usedSkillPoints = 0;
        learnedSkills.Clear();
        Debug.Log("모든 스킬을 초기화했습니다.");
        // 캐릭터에게 적용된 모든 스킬 효과를 제거하는 로직이 필요합니다.
        // 예: GetComponent<PlayerStats>().RemoveAllSkillBonuses();
        OnSkillChanged.Invoke();
    }
}