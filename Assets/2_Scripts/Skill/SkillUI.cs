// SkillUI.cs
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // 마우스 이벤트를 위해 추가

public class SkillUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("연결")]
    public Skill assignedSkill;
    public Image skillIconImage;
    public Button skillButton;
    public Text levelText; // 스킬 레벨 표시용

    private CharacterSkillTree skillTree;

    void Start()
    {
        skillTree = FindAnyObjectByType<CharacterSkillTree>();
        skillButton.onClick.AddListener(() => skillTree.LearnSkill(assignedSkill));
        skillTree.OnSkillChanged.AddListener(UpdateUI);
        UpdateUI();
    }

    public void UpdateUI()
    {
        int currentLevel = skillTree.learnedSkills.ContainsKey(assignedSkill) ? skillTree.learnedSkills[assignedSkill] : 0;

        // 레벨 텍스트 업데이트
        if (assignedSkill is PassiveSkill pSkill)
        {
            levelText.text = $"{currentLevel} / {pSkill.maxLevel}";
        }
        else
        {
            levelText.text = $"{currentLevel} / 1";
        }

        // 상태에 따른 시각적 변화
        if (currentLevel > 0)
        {
            // 습득 완료
            skillIconImage.color = Color.white;
            // 만렙이면 더이상 클릭 불가
            if (assignedSkill is PassiveSkill ps && currentLevel >= ps.maxLevel)
            {
                skillButton.interactable = false;
            }
            else if (assignedSkill is TraitSkill)
            {
                skillButton.interactable = false;
            }
        }
        else
        {
            // 습득 전
            skillIconImage.color = Color.gray;
            skillButton.interactable = IsUnlockable();

            if (IsUnlockable())
            {
                // 습득 가능 (테두리 발광 효과 처리)
            }
        }
    }

    // 이 스킬을 현재 배울 수 있는지 확인
    private bool IsUnlockable()
    {
        if (skillTree.totalSkillPoints - skillTree.usedSkillPoints < 1 && assignedSkill is PassiveSkill) return false;

        if (assignedSkill is TraitSkill trait)
        {
            int pointsInTier = skillTree.GetPointsInTier(trait.requiredTier);
            return pointsInTier >= trait.requiredPointsInTier;
        }

        // 2티어 이상의 패시브 스킬은 이전 티어 특성 스킬을 배웠는지 확인
        for (int i = 1; i < skillTree.tiers.Length; i++)
        {
            foreach (var pSkill in skillTree.tiers[i].passiveSkills)
            {
                if (pSkill == assignedSkill)
                {
                    // 이전 티어의 특성 스킬을 배웠는지 확인
                    TraitSkill prevTrait = skillTree.tiers[i - 1].traitSkill;
                    if (!skillTree.learnedSkills.ContainsKey(prevTrait))
                    {
                        return false;
                    }
                }
            }
        }

        return true; // 1티어 패시브는 항상 해금 가능
    }

    // 마우스가 아이콘 위에 올라갔을 때 (툴팁 표시)
    public void OnPointerEnter(PointerEventData eventData)
    {
        // TooltipManager.Show(assignedSkill.skillName, assignedSkill.description);
    }

    // 마우스가 아이콘을 빠져나갔을 때 (툴팁 숨기기)
    public void OnPointerExit(PointerEventData eventData)
    {
        // TooltipManager.Hide();
    }
}