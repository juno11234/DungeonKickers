using UnityEngine;
using UnityEngine.AI;

public abstract class PlayerUnit : MonoBehaviour, IFighter
{

    [SerializeField] protected PlayerDataSO playerSO;
    [SerializeField] protected ActiveSkillSO activeSO;
    [SerializeField] private GameObject selectedMarker;


    private Collider _myCollider;
    private Animator _myAnimator;
    private NavMeshAgent agent;
    private Detector detector;
    public Collider MainCollider => _myCollider;
    public GameObject GameObject => gameObject;

    public int HP => _hp;
    private int _hp;
    private float attackRange;
    private float attackSpeed;
    private IFighter _targetMonster;
    private float originalAttackAnimLength; // 애니메이션의 원래 길이
    private float attackAniSpeed;
    protected int guard = 1;
    bool isMoving = false;
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        _myCollider = GetComponent<Collider>();
        _myAnimator = GetComponent<Animator>();
        detector = GetComponentInChildren<Detector>();
        selectedMarker.SetActive(false);

        agent.speed = playerSO.moveSpeed;
        _hp = playerSO.hp;
        attackRange = playerSO.attackRange;
        detector.coll.radius = attackRange;
        attackSpeed = playerSO.attackSpeed;

        originalAttackAnimLength = GetAnimationLength("Attack");
        float desiredDuration = 1f / attackSpeed;
        attackAniSpeed = originalAttackAnimLength / desiredDuration;

        detector.OnTargetFind += AttackTargetSet;
    }

    // Update 메서드 추가
    // 매 프레임마다 공격 상태를 확인합니다.
    private void Update()
    {
        if (agent.velocity.magnitude > 0)
        {
            _myAnimator.SetFloat("Speed", 1f);
        }
        else
        {
            _myAnimator.SetFloat("Speed", 0f);
        }

        AttackOrChase();
    }
    public abstract void Skill();
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
    private void AttackOrChase()
    {
        if (_targetMonster != null)
        {
            transform.LookAt(_targetMonster.GameObject.transform.position, Vector3.up);
            // 대상과의 거리 계산
            float distance = Vector3.Distance(transform.position, _targetMonster.GameObject.transform.position);

            // 사거리 안에 들어오면 공격 시작
            if (distance <= attackRange)
            {
                // 이동을 멈춥니다.
                agent.ResetPath();
                _myAnimator.SetFloat("attackSpeed", attackAniSpeed);
            }
            else
            {
                Move(_targetMonster.GameObject.transform.position);
            }

        }
    }
    //힐러는 아군 힐로 수정
    public virtual void AttackEvent()
    {
        Debug.Log(gameObject.name + "Attack");
        // CombatSystem에 공격 이벤트 추가
        CombatEvent combatEvents = new()
        {
            Sender = this,
            Receiver = _targetMonster,
            Damage = playerSO.attackDamage,
            Collider = _targetMonster.MainCollider
        };
        CombatSystem.Instance.AddInGameEvent(combatEvents);
    }

    public void AttackTargetSet(IFighter target)
    {
        _targetMonster = target;
    }

    public void Selected()
    {
        selectedMarker.SetActive(true);
    }

    public void CanceledSelected()
    {
        selectedMarker.SetActive(false);
    }

    public void Move(Vector3 position)
    {
        // 이동 명령 시 도착지점에 도달할때까지는 공격 무시
        _myAnimator.SetFloat("attackSpeed", 0);
        agent.destination = position;
    }
    public void MoveAttackGround(Vector3 position)
    {
        detector.EventCloseDict();

        if (_targetMonster == null)
        {
            _myAnimator.SetFloat("attackSpeed", 0);
            agent.destination = position;
        }
    }

    public void MonsterTargetCancel()
    {
        // 몬스터가 죽었을 때 대상을 초기화하는 메서드
        if (_targetMonster != null)
        {
            _targetMonster = null;
        }
    }

    public void TakeDamage(CombatEvent combatEvent)
    {
        _hp -= combatEvent.Damage / guard;
        if (_hp <= 0)
        {
            //죽음
        }
    }

}