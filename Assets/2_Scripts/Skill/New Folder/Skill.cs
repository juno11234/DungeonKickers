using UnityEngine;

public abstract class Skill : ScriptableObject
{
    [Header("공통 정보")]
    public string skillName;
    [TextArea]
    public string description;
    public Sprite skillIcon;
    public SkillOwner owner;
    // 스킬의 효과를 캐릭터에게 적용하는 추상 메소드

    public abstract void ApplyEffect(PlayerUnit unit, int currentLevel);
}
