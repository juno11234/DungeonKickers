using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public abstract class TargetingSkillerBase : PlayerUnit
{
    public event Action<Texture2D, float> cursorChangeEvent;
    [SerializeField] private float skillRange = 10f;
    [SerializeField] private GameObject rangeCircle;
    public Vector3 TargetPos { get; set; }

    protected void InvokeCursorChange(Texture2D skillCursor)
    {
        rangeCircle.transform.localScale = new Vector3(2 * skillRange, 2 * skillRange, 0f);
        rangeCircle.SetActive(true);
        cursorChangeEvent?.Invoke(skillCursor, skillRange);
    }

    protected void RangeCheck()
    {
        float distance = Vector3.Distance(transform.position, TargetPos);
        rangeCircle.SetActive(false);
        if (distance <= skillRange)
        {
            // 사거리 내 → 바로 발사
            CastSkill();
        }
        else
        {
            // 사거리 밖 → 이동 후 발사
            Vector3 dir = (TargetPos - transform.position).normalized;
            Vector3 movePos = TargetPos - dir * (skillRange); // 약간 여유

            Move(movePos); // PlayerUnit의 이동 메서드 사용 타겟에서 지금위치 방향으로 fireBallRange거리를 뺀 위치

            // 이동 완료 감지 후 발사하도록 코루틴 실행
            StartCoroutine(CastAfterReach(movePos));
        }
    }

    private IEnumerator CastAfterReach(Vector3 movePos)
    {
        // 도착할 때까지 대기
        while (Vector3.Distance(transform.position, movePos) > 0.3f)
            yield return null;

        CastSkill();
    }

    protected abstract void CastSkill();

    public abstract void CursorChange();
}
