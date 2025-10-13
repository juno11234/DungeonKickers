using UnityEngine;

public class FireBall : MonoBehaviour
{
    private Vector3 targetPos;
    [SerializeField] private float speed = 10f;

    // 초기화용 메서드 (생성자 대신 사용)
    public void Init(Vector3 target)
    {
        targetPos = target;
        targetPos.y = 1;
    }

    void Update()
    {
        // 타겟 방향으로 이동 (간단 예시)
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        // 타겟 도착 시 제거 (옵션)
        if (Vector3.Distance(transform.position, targetPos) < 0.1f)
        {
            //Destroy(gameObject);
        }
    }
}
