using System.Collections;
using UnityEngine;

public class Player_Rogue : PlayerUnit
{
    [SerializeField] private float buffTime = 10;
    [SerializeField] private float attackSpeedBuffVal = 0.5f;
    [SerializeField] private GameObject buffEffect;
    public override void Skill()
    {
        //이속 공속 증가 코루틴
        base.Skill();
        StartCoroutine(SpeedBuff());
        CombatEvent combatEvents = new()
        {
            Sender = this,
            Receiver = this,
            Damage = 10,
            Collider = MainCollider
        };
        CombatSystem.Instance.AddInGameEvent(combatEvents);
    }
    IEnumerator SpeedBuff()
    {
        buffEffect.SetActive(true);
        SpeedSet(activeSkillSO.value, attackSpeedBuffVal);

        yield return new WaitForSeconds(buffTime);

        buffEffect.SetActive(false);
        SpeedSet(0, 0);
    }
}
