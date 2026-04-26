using System.Collections;
using UnityEngine;

public class Player_Warrior : PlayerUnit
{
    [SerializeField] private float buffTime = 10;
    [SerializeField] private GameObject buffEffect;
    public override void Skill()
    {
        //방어력 증가 버프 코루틴
        base.Skill();
        StartCoroutine(DefBuff());
    }
    IEnumerator DefBuff()
    {
        _stats.guard += activeSkillSO.value;
        buffEffect.SetActive(true);
        InvokedStatChange();
        yield return new WaitForSeconds(buffTime);

        _stats.guard -= activeSkillSO.value;
        buffEffect.SetActive(false);
        InvokedStatChange();
    }
}
