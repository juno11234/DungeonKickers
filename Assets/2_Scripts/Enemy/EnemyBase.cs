using System;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyBase : MonoBehaviour, IFighter
{
    [SerializeField] MonsterDataSo monsterSO;
    public Collider MainCollider => _collider;
    public GameObject GameObject => gameObject;
    public bool OnDie => isDead;

    private Detector detector;
    private Collider _collider;
    private Animator animator;
    private NavMeshAgent agent;
    private IFighter target;
    private float attackRange;
    private float attackSpeed;
    private float originalAttackAnimLength;
    private float attackAniSpeed;
    private int hp;
    private bool isDead = false;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        _collider = GetComponent<Collider>();
        agent = GetComponent<NavMeshAgent>();
        detector = GetComponentInChildren<Detector>();

        hp = monsterSO.hp;
        agent.speed = monsterSO.moveSpeed;
        attackRange = monsterSO.attackRange;
        attackSpeed = monsterSO.attackSpeed;
        detector.coll.radius = 12;

        originalAttackAnimLength = GetAnimationLength("Attack");
        float desiredDuration = 1f / attackSpeed;
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
        // AnimatorController가 있는지 확인
        if (animator == null || animator.runtimeAnimatorController == null)
        {
            Debug.LogError("Animator 또는 AnimatorController가 할당되지 않았습니다.");
            return 0f;
        }

        // 모든 상태를 순회하며 "Attack" 상태를 찾습니다.
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
            // 대상과의 거리 계산
            float distance = Vector3.Distance(transform.position, target.GameObject.transform.position);

            // 사거리 안에 들어오면 공격 시작
            if (distance <= attackRange)
            {
                // 이동을 멈춥니다.
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
        // 이동 명령 시 공격 대상 초기화
        animator.SetFloat("attackSpeed", 0);
        agent.destination = position;
    }

    public void TakeDamage(CombatEvent combatEvent)
    {
        if (isDead) return;
        hp -= combatEvent.Damage;
        if (hp <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        CombatSystem.Instance.ReMoveDictionary(this);
        animator.SetTrigger("Die");
        detector.DictionaryReset();
        _collider.enabled = false;
        agent.enabled = false;
    }

}