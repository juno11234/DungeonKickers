using System;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyBase : MonoBehaviour, IFighter
{
    [SerializeField] MonsterDataSo monsterSO;
    [SerializeField] GameObject miniMap;
    public Collider MainCollider => _collider;
    public GameObject GameObject => gameObject;
    public bool OnDie => isDead;

    private Detector detector;
    private Collider _collider;
    private Animator animator;
    private NavMeshAgent agent;
    private IFighter target;

    // 기존 개별 능력치를 UnitStats 구조체로 대체
    private UnitStats _stats;

    private float originalAttackAnimLength;
    private float attackAniSpeed;
    private bool isDead = false;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        _collider = GetComponent<Collider>();
        agent = GetComponent<NavMeshAgent>();
        detector = GetComponentInChildren<Detector>();

        // MonsterDataSO로부터 UnitStats 초기화
        _stats = new UnitStats(
            monsterSO.hp,
            monsterSO.attackRange,
            monsterSO.attackSpeed,
            0, // 몬스터는 방어력이 없으므로 0으로 설정
            monsterSO.moveSpeed,
            monsterSO.attackDamage,
            monsterSO.name
        );

        agent.speed = _stats.moveSpeed;
        detector.coll.radius = 12;

        originalAttackAnimLength = GetAnimationLength("Attack");
        float desiredDuration = 1f / _stats.attackSpeed;
        attackAniSpeed = originalAttackAnimLength / desiredDuration;

        detector.OnTargetFind += AttackTargetSet;

        CombatSystem.Instance.RegisterMonster(this);
    }

    private void Update()
    {
        if (agent.velocity.magnitude > 0)
        {
            animator.SetFloat("Speed", 1f);
        }
        else
        {
            animator.SetFloat("Speed", 0f);
        }

        AttackOrChase();
    }

    private float GetAnimationLength(string stateName)
    {
        if (animator == null || animator.runtimeAnimatorController == null)
        {
            Debug.LogError("Animator 또는 AnimatorController가 할당되지 않았습니다.");
            return 0f;
        }

        foreach (var state in animator.runtimeAnimatorController.animationClips)
        {
            if (state.name.Contains(stateName))
            {
                return state.length;
            }
        }

        Debug.LogError($"'{stateName}' 애니메이션 클립을 찾을 수 없습니다.");
        return 0f;
    }

    private void AttackOrChase()
    {
        if (target != null && isDead == false)
        {
            if (target.OnDie)
            {
                animator.SetFloat("attackSpeed", 0);
                detector.DictionaryRemove(target.MainCollider);
                target = null;
                detector.EventCloseDict();
                return;
            }
            transform.LookAt(target.GameObject.transform.position, Vector3.up);
            float distance = Vector3.Distance(transform.position, target.GameObject.transform.position);

            // attackRange 대신 _stats.attackRange 사용
            if (distance <= _stats.attackRange)
            {
                agent.ResetPath();
                animator.SetFloat("attackSpeed", attackAniSpeed);
            }
            else
            {
                Move(target.GameObject.transform.position);
            }
        }
    }

    public void AttackTargetSet(IFighter targetSet)
    {
        target = targetSet;
    }

    public void AttackEvent()
    {
        CombatEvent combatEvents = new()
        {
            Sender = this,
            Receiver = target,
            Damage = monsterSO.attackDamage,
            Collider = target.MainCollider
        };
        CombatSystem.Instance.AddInGameEvent(combatEvents);
    }

    public void Move(Vector3 position)
    {
        animator.SetFloat("attackSpeed", 0);
        agent.destination = position;
    }

    public void TakeHeal(HealEvent heal)
    {

    }
    public void TakeDamage(CombatEvent combatEvent)
    {
        if (isDead) return;

        // hp 대신 _stats.hp 사용
        _stats.hp -= combatEvent.Damage;
        if (_stats.hp <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        EXPEvnet expEvent = new()
        {
            Exp=monsterSO.EXP
        };
        CombatSystem.Instance.AddInGameEvent(expEvent);

        isDead = true;
        CombatSystem.Instance.ReMoveDictionary(this);
        animator.SetTrigger("Die");
        detector.DictionaryReset();
        _collider.enabled = false;
        agent.enabled = false;
        miniMap.SetActive(false);
    }
}