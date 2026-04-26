using System;
using UnityEngine;
using UnityEngine.AI;
[System.Serializable]
public struct UnitStats
{
    public string name;
    public int maxHp;
    public int hp;
    public int damage;
    public float attackRange;
    public float attackSpeed;
    public int guard;
    public float moveSpeed;

    // 모든 필드를 초기화하는 생성자
    public UnitStats(int hp, float attackRange, float attackSpeed, int guard, float moveSpeed, int damage, string name)
    {
        this.name = name;
        this.damage = damage;
        this.maxHp = hp;
        this.hp = hp;
        this.attackRange = attackRange;
        this.attackSpeed = attackSpeed;
        this.guard = guard;
        this.moveSpeed = moveSpeed;
    }
}
public abstract class PlayerUnit : MonoBehaviour, IFighter
{
    protected enum EUnitState
    {
        Idle,       // 대기
        Moving,     // 일반 이동 (땅 클릭)
        Chasing,    // 적 추적
        Attacking,  // 공격
        Skill,      // 스킬 시전
        Patrolling, // 정찰
        Die         // 사망
    }

    [SerializeField] protected PlayerDataSO playerSO;
    [SerializeField] protected ActiveSkillSO activeSkillSO;
    [SerializeField] private GameObject selectedMarker;
    [SerializeField] private int skillIndex;
    [SerializeField] private RectTransform canvasRect;
    [SerializeField] private HPBar hpBarPrefab;
    [SerializeField] private GameObject miniMap;

    public event Action<int, int> HpChanged;
    public event Action<int, int> StatChanged;
    protected EUnitState _currentState;

    public Collider MainCollider => _myCollider;
    public GameObject GameObject => gameObject;
    public Detector Detector => detector;
    public bool OnDie => isDead;
    public bool SkillAble => skillAble;

    public UnitStats Stats => _stats;
    public int CurrentHp => _stats.hp;
    public int MaxHp => _stats.maxHp;

    private Collider _myCollider;
    private Animator _myAnimator;
    private NavMeshAgent agent;
    private Detector detector;

    protected UnitStats _stats;

    private IFighter _targetMonster;
    private float originalAttackAnimLength; // 애니메이션의 원래 길이
    private float attackAniSpeed;
    private bool isDead = false;
    private Vector3 _destination; // 이동 목표 지점
    private Vector3 _patrolStartPos, _patrolEndPos; // 정찰 시작/끝 지점
    private bool _isPatrollingForward; // [추가] 정찰 방향 플래그

    private float skillCool;
    public float skillTimer;
    protected bool skillAble = true;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        _myCollider = GetComponent<Collider>();
        _myAnimator = GetComponent<Animator>();
        detector = GetComponentInChildren<Detector>();
        selectedMarker.SetActive(false);

        _stats = playerSO.GetUnitStats();
        skillCool = activeSkillSO.coolTime;
        detector.coll.radius = 12;

        originalAttackAnimLength = GetAnimationLength("Attack");
        SpeedSet(0, 0);

        detector.OnTargetFind += AttackTargetSet;
        _currentState = EUnitState.Idle;

        SpawnUnit(this);
    }
    void SpawnUnit(PlayerUnit unit)
    {
        var bar = Instantiate(hpBarPrefab, canvasRect);
        bar.Init(unit, canvasRect);
    }

    protected void SpeedSet(float speedBuff, float atkSpeedBuff)
    {
        agent.speed = _stats.moveSpeed + speedBuff;

        float desiredDuration = 1f / (_stats.attackSpeed + atkSpeedBuff);
        attackAniSpeed = originalAttackAnimLength / desiredDuration;
    }

    // Update 메서드 추가
    // 매 프레임마다 공격 상태를 확인합니다.
    private void Update()
    {
        if (skillAble == false)
        {
            skillTimer -= Time.deltaTime;
        }

        if (skillTimer <= 0f && skillAble == false)
        {
            skillAble = true;
        }

        // 상태에 따라 다른 로직을 수행하도록 변경
        switch (_currentState)
        {
            case EUnitState.Idle:
                UpdateIdle();
                break;
            case EUnitState.Moving:
                UpdateMoving();
                break;
            case EUnitState.Chasing:
                UpdateChasing();
                break;
            case EUnitState.Attacking:
                UpdateAttacking();
                break;
            case EUnitState.Patrolling:
                UpdatePatrolling();
                break;
        }

        // 애니메이터 속도 설정 (모든 상태 공통)
        _myAnimator.SetFloat("Speed", agent.velocity.magnitude > 0.1f ? 1f : 0f);
    }

    public bool SkillCheck(int index)
    {
        if (skillIndex == index)
        {
            return skillAble;
        }
        else
        {
            return false;
        }
    }

    #region State Update Methods
    private void UpdateIdle()
    {
        // Idle 상태에서는 자동으로 주변의 적을 찾도록 Detector를 켤 수 있습니다.
        OnDetector();
    }

    private void UpdateMoving()
    {
        // 목표 지점에 도착했는지 확인
        if (agent.pathPending == false && agent.remainingDistance <= agent.stoppingDistance)
        {
            SetState(EUnitState.Idle);
        }
    }

    private void UpdateChasing()
    {
        if (_targetMonster == null || _targetMonster.OnDie)
        {
            IFighter newTarget = detector.FindClosestTarget();
            if (newTarget != null)
            {
                AttackTargetSet(newTarget);
            }
            else
            {
                SetState(EUnitState.Idle);
            }
            return;
        }

        // 2. 거리 계산 최적화 (sqrMagnitude 사용)
        float sqrDistance = (transform.position - _targetMonster.GameObject.transform.position).sqrMagnitude;
        float sqrAttackRange = _stats.attackRange * _stats.attackRange;

        if (sqrDistance <= sqrAttackRange)
        {
            SetState(EUnitState.Attacking);
        }
        else
        {
            // 목표를 계속 따라가도록 매 프레임 목적지 갱신
            agent.SetDestination(_targetMonster.GameObject.transform.position);
        }
    }

    private void UpdateAttacking()
    {
        if (_targetMonster == null || _targetMonster.OnDie)
        {
            IFighter newTarget = detector.FindClosestTarget();
            if (newTarget != null)
            {
                AttackTargetSet(newTarget);
            }
            else
            {
                SetState(EUnitState.Idle);
            }
            return;
        }

        agent.ResetPath(); // 공격 중에는 이동 중지

        // 3. 부드러운 회전
        SmoothLookAt(_targetMonster.GameObject.transform.position);

        float sqrDistance = (transform.position - _targetMonster.GameObject.transform.position).sqrMagnitude;
        float sqrAttackRange = _stats.attackRange * _stats.attackRange;

        if (sqrDistance > sqrAttackRange)
        {
            SetState(EUnitState.Chasing);
        }
    }

    // 4. 정찰 기능 구현
    private void UpdatePatrolling()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            // 끝 지점으로 가던 중이었다면 (그리고 도착했다면)
            if (_isPatrollingForward)
            {
                // 다음 목적지는 시작 지점
                agent.SetDestination(_patrolStartPos);
            }
            // 시작 지점으로 가던 중이었다면 (그리고 도착했다면)
            else
            {
                // 다음 목적지는 끝 지점
                agent.SetDestination(_patrolEndPos);
            }

            // 방향 플래그를 뒤집어 줍니다.
            _isPatrollingForward = !_isPatrollingForward;
        }
    }
    #endregion
    private void SetState(EUnitState newState)
    {
        if (_currentState == newState) return;

        // 이전 상태를 나갈 때의 처리
        switch (_currentState)
        {
            case EUnitState.Attacking:
                _myAnimator.SetFloat("attackSpeed", 0);
                break;
        }

        _currentState = newState;

        // 새로운 상태에 진입할 때의 처리
        switch (newState)
        {
            case EUnitState.Idle:
                agent.ResetPath();
                OnDetector();
                break;
            case EUnitState.Moving:
                _targetMonster = null;
                OffDetector();
                agent.SetDestination(_destination); // 여기서 _destination 사용
                break;
            case EUnitState.Chasing:
                OnDetector();
                agent.SetDestination(_targetMonster.GameObject.transform.position);
                break;
            case EUnitState.Attacking:
                OnDetector();
                _myAnimator.SetFloat("attackSpeed", attackAniSpeed);
                break;
            case EUnitState.Patrolling:
                _targetMonster = null;
                OnDetector();
                agent.SetDestination(_patrolEndPos);
                _isPatrollingForward = true; // [수정] 정찰 시작 시, 끝 지점으로 향한다고 설정
                break;
            case EUnitState.Die:
                detector.DictionaryReset();
                _myAnimator.SetTrigger("Die");
                isDead = true;
                agent.enabled = false;
                miniMap.gameObject.SetActive(false);
                break;
        }
    }
    private void SmoothLookAt(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, agent.angularSpeed * Time.deltaTime);
        }
    }

    public virtual void Skill()
    {
        skillTimer = skillCool;
        skillAble = false;
    }

    private float GetAnimationLength(string stateName)
    {
        // AnimatorController가 있는지 확인
        if (_myAnimator == null || _myAnimator.runtimeAnimatorController == null)
        {
            Debug.LogError("Animator 또는 AnimatorController가 할당되지 않았습니다.");
            return 0f;
        }

        // 모든 상태를 순회하며 "Attack" 상태를 찾습니다.
        foreach (var state in _myAnimator.runtimeAnimatorController.animationClips)
        {
            if (state.name.Contains(stateName))
            {
                return state.length;
            }
        }

        Debug.LogError($"'{stateName}' 애니메이션 클립을 찾을 수 없습니다.");
        return 0f;
    }

    //힐러는 아군 힐로 수정
    public virtual void AttackEvent()
    {
        // CombatSystem에 공격 이벤트 추가
        CombatEvent combatEvents = new()
        {
            Sender = this,
            Receiver = _targetMonster,
            Damage = _stats.damage,
            Collider = _targetMonster.MainCollider
        };
        CombatSystem.Instance.AddInGameEvent(combatEvents);
    }

    public void AttackTargetSet(IFighter target)
    {
        if (target == null || target.OnDie) return;

        _targetMonster = target;

        // 이미 추격 중인 대상이 동일하다면 목적지만 갱신
        if (_currentState == EUnitState.Chasing)
        {
            agent.SetDestination(_targetMonster.GameObject.transform.position);
        }
        else
        {
            SetState(EUnitState.Chasing);
        }
    }

    public void Selected()
    {
        selectedMarker.SetActive(true);
    }

    public void deSelected()
    {
        selectedMarker.SetActive(false);
    }

    public void Move(Vector3 position)
    {
        // 1. 목표 지점 데이터를 먼저 갱신합니다.
        _destination = position;

        // 2. 만약 이미 이동 중이라면, 목적지만 갱신하고 끝냅니다.
        if (_currentState == EUnitState.Moving)
        {
            agent.SetDestination(_destination);
        }
        // 3. 다른 상태였다면, 상태를 Moving으로 전환합니다.
        else
        {
            SetState(EUnitState.Moving);
        }
    }
    //
    public void AttackGround(Vector3 position)
    {
        detector.EventCloseDict();

        if (_targetMonster == null)
        {
            _myAnimator.SetFloat("attackSpeed", 0);
            agent.destination = position;
        }
    }

    public void Patrol(Vector3 pos)
    {
        _patrolStartPos = transform.position;
        _patrolEndPos = pos;
        SetState(EUnitState.Patrolling);
    }

    public void MonsterTargetCancel()
    {
        // 몬스터가 죽었을 때 대상을 초기화하는 메서드
        if (_targetMonster != null)
        {
            _targetMonster = null;
        }
    }
    public void OffDetector()
    {
        detector.gameObject.SetActive(false);
    }
    public void OnDetector()
    {
        detector.gameObject.SetActive(true);
    }
    public void TakeHeal(HealEvent healEvent)
    {
        if (isDead) return;

        _stats.hp += healEvent.Heal;
        if (_stats.hp > _stats.maxHp)
        {
            _stats.hp = _stats.maxHp;
        }
        HpChanged?.Invoke(_stats.hp, _stats.maxHp);
    }
    public void TakeDamage(CombatEvent combatEvent)
    {
        if (isDead) return;
        int dmg = combatEvent.Damage - _stats.guard;
        if (dmg > 0)
        {
            _stats.hp -= dmg;

        }
        else
        {
            _stats.hp -= 1;
        }

        if (_stats.hp <= 0)
        {
            SetState(EUnitState.Die); // [수정] Die() 직접 호출 대신 SetState 호출
        }
        HpChanged?.Invoke(_stats.hp, _stats.maxHp);
    }
    public GameObject GetSkillIconUI()
    {
        return activeSkillSO.skillIcon;
    }
    public int GetSkillID()
    {
        return activeSkillSO.id;
    }
    public float GetSkillCool()
    {
        return activeSkillSO.coolTime;
    }
    public Sprite GetPortrait()
    {
        return playerSO.portrait;
    }
    protected void InvokedStatChange()
    {
        StatChanged?.Invoke(_stats.damage, _stats.guard);
    }
    public void ApplyPassiveSkill(BuffType type, int currentLevel)
    {
        switch (type)
        {
            case BuffType.Hp:
                _stats.maxHp += 50 * currentLevel;
                _stats.hp = _stats.maxHp;
                break;
            case BuffType.Atk:
                _stats.damage += 5 * currentLevel;
                break;
            case BuffType.Ats:
                _stats.attackSpeed += 0.1f * currentLevel;
                break;
            case BuffType.Def:
                _stats.guard += 2 * currentLevel;
                break;
            case BuffType.Speed:
                _stats.moveSpeed += 0.2f * currentLevel;
                SpeedSet(0, 0);
                break;
        }
        InvokedStatChange();
        
    }
}