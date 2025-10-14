using System.Collections;
using UnityEngine;

public class Player_Priest : TargetingSkillerBase
{
    [SerializeField] private GameObject healEffectPrefab;
    [SerializeField] private Texture2D skillCursor;
    public PlayerUnit TargetPlayer { get; set; }

    public override void CursorChange()
    {
        InvokeCursorChange(skillCursor, 0f, false);
    }
    protected override void CastSkill()
    {
        if (TargetPlayer == null) return;

        GameObject effect = Instantiate(
            healEffectPrefab,
            TargetPlayer.transform.position,
            Quaternion.identity,
            TargetPlayer.transform
        );

        // 이펙트 위치 미세 조정 (ex. 머리 위로 올리고 싶을 때)
        effect.transform.localPosition = new Vector3(0, 1.5f, 0);

        TargetPlayer.TakeHeal(); // 힐 함수 (직접 구현 필요)
        Skill(); // 쿨타임 적용
    }
    public void StartFollowAndCast(PlayerUnit target)
    {
        TargetPlayer = target;
        StopAllCoroutines(); // 혹시 이전 추적 중이면 중단
        StartCoroutine(FollowAndCastCoroutine());
    }

    private IEnumerator FollowAndCastCoroutine()
    {
        while (TargetPlayer != null)
        {
            float distance = Vector3.Distance(transform.position, TargetPlayer.transform.position);

            // 사거리 안이라면 시전
            if (distance <= skillDistance)
            {
                CastSkill();
                yield break; // 시전 완료 후 코루틴 종료
            }

            // 사거리 밖이면 계속 타겟을 향해 이동 (목표 위치 갱신)
            Move(TargetPlayer.transform.position);

            yield return new WaitForSeconds(0.1f); // 너무 자주 갱신하면 비효율 → 0.1초 간격으로 갱신
        }
    }
}
