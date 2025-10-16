using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private SphereCollider coll;
    [SerializeField] private GameObject effect;
    [SerializeField] private GameObject boomRadius;

    private HashSet<IFighter> damagedEnemies = new HashSet<IFighter>();

    private Vector3 targetPos;
    private int damage;
    private PlayerUnit mage;
    private float timer;
    // 초기화용 메서드 (생성자 대신 사용)

    public void Init(PlayerUnit owner, int Damage, Vector3 target, float radius)
    {
        mage = owner;
        damage = Damage;
        targetPos = target;
        boomRadius.transform.localScale = new Vector3(2 * radius, 2 * radius, 1f);
        targetPos.y = 1;
        coll.radius = radius;
        coll.enabled = false;
        effect.SetActive(false);
        boomRadius.SetActive(false);
    }

    void Update()
    {
        // 타겟 방향으로 이동 (간단 예시)
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        // 타겟 도착 시 제거 (옵션)
        if (Vector3.Distance(transform.position, targetPos) < 0.1f)
        {
            effect.SetActive(true);
            boomRadius.SetActive(true);
            coll.enabled = true;
            timer += Time.deltaTime;
        }
        if (timer >= 2f)
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;

        IFighter enemy = CombatSystem.Instance.GetMonsterOrNull(other);

        //  이미 데미지를 준 적이면 무시
        if (damagedEnemies.Contains(enemy)) return;
        //  새로운 적이면 HashSet에 추가 후 데미지 처리
        damagedEnemies.Add(enemy);

        CombatEvent combatEvents = new()
        {
            Sender = mage,
            Receiver = enemy,
            Damage = damage,
            Collider = other
        };

        CombatSystem.Instance.AddInGameEvent(combatEvents);
    }
}
