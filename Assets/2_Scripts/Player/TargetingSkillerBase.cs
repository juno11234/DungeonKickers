using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public abstract class TargetingSkillerBase : PlayerUnit
{
    public event Action<Texture2D, float, bool> cursorChangeEvent;
    [SerializeField] protected float skillDistance = 10f;
    [SerializeField] private GameObject rangeCircle;
    public Vector3 TargetPos { get; set; }

    protected void InvokeCursorChange(Texture2D skillCursor, float skillRange, bool wide)
    {
        rangeCircle.transform.localScale = new Vector3(2 * skillDistance, 2 * skillDistance, 0f);
        rangeCircle.SetActive(true);
        cursorChangeEvent?.Invoke(skillCursor, skillRange, wide);
    }

    public void RangeCheck()
    {
        float distance = Vector3.Distance(transform.position, TargetPos);
        rangeCircle.SetActive(false);
        if (distance <= skillDistance)
        {
            // 사거리 내 → 바로 발사
            CastSkill();
        }
        else
        {
            // 사거리 밖 → 이동 후 발사
            Vector3 dir = (TargetPos - transform.position).normalized;
            Vector3 movePos = TargetPos - dir * (skillDistance); // 약간 여유

            Move(movePos); // PlayerUnit의 이동 메서드 사용 타겟에서 지금위치 방향으로 fireBallRange거리를 뺀 위치

            // 이동 완료 감지 후 발사하도록 코루틴 실행
            castCoroutine = StartCoroutine(CastAfterReach(movePos));
        }
    }
    private Coroutine castCoroutine;
    public void TargetCancel(bool lmb)
    {
        rangeCircle.SetActive(false);
        if (castCoroutine != null && lmb == false)
        {
            StopCoroutine(castCoroutine);
        }
    }
    private IEnumerator CastAfterReach(Vector3 movePos)
    {
        // 도착할 때까지 대기
        while (Vector3.Distance(transform.position, movePos) > 0.3f)
            yield return null;

        CastSkill();
    }
    public override void Skill()
    {
        base.Skill();
    }

    protected abstract void CastSkill();

    public abstract void CursorChange();
}
