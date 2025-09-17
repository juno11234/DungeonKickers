using UnityEngine;
using UnityEngine.VFX;

public class FighterAttakcSMB : StateMachineBehaviour
{
    [Range(0f, 1f)]
    public float startNormalizedTime = 0f;

    private bool passStartNormalizedTime;

    IFighter fighter;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        passStartNormalizedTime = false;
        fighter = animator.gameObject.GetComponent<IFighter>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 정규화된 시간의 소수점 부분만 사용하여 애니메이션 루프 내의 시간을 확인합니다.
        float normalizedTimeInLoop = stateInfo.normalizedTime % 1;

        // 공격 이벤트가 이미 발생했고, 다음 루프의 시작 지점(NormalizedTime이 0에 가까워졌을 때)에 도달하면
        // passStartNormalizedTime을 초기화하여 다음 루프에서 공격 이벤트가 발생할 수 있도록 준비합니다.
        if (passStartNormalizedTime && normalizedTimeInLoop < startNormalizedTime)
        {
            passStartNormalizedTime = false;
        }

        // 공격 시작 지점을 통과하지 않았고, 현재 시간이 시작 지점을 넘었다면 공격 이벤트를 발생시킵니다.
        if (passStartNormalizedTime==false && normalizedTimeInLoop >= startNormalizedTime)
        {
            fighter.AttackEvent();
            passStartNormalizedTime = true;
        }
    }

    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{

    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
