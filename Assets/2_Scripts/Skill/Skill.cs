using UnityEngine;

public abstract class Skill : ScriptableObject
{
    [Header("공통 정보")]
    public string skillName;
    [TextArea]
    public string description;
    public Sprite skillIcon;

    // 스킬의 효과를 캐릭터에게 적용하는 추상 메소드
    // 상속받는 클래스(PassiveSkill, TraitSkill)에서 반드시 이 메소드를 구현해야 함
    public abstract void ApplyEffect(GameObject character);
}
