using UnityEngine;
using UnityEngine.AI;

public abstract class PlayerUnit : MonoBehaviour, IFighter
{

    [SerializeField] protected PlayerDataSO playerSO;
    [SerializeField] private GameObject selectedMarker;
    private Collider _myCollider;
    private Animator _myAnimator;
    private NavMeshAgent agent;

    public Collider MainCollider => _myCollider;
    public GameObject GameObject => gameObject;

    private int _hp;
    private float attackRange;
    private float attackSpeed;
    private float attackTimer;
    private IFighter _targetMonster;
    private float originalAttackAnimLength; // 애니메이션의 원래 길이
    private bool isInitialized = false;
    float attackAniSpeed;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        _myCollider = GetComponent<Collider>();
        _myAnimator = GetComponent<Animator>();

        selectedMarker.SetActive(false);

        agent.speed = playerSO.moveSpeed;
        _hp = playerSO.hp;
        attackRange = playerSO.attackRange;
        attackSpeed = playerSO.attackSpeed;
        attackTimer = playerSO.attackSpeed;

        originalAttackAnimLength = GetAnimationLength("Attack");
        float desiredDuration = 1f / attackSpeed;
        attackAniSpeed = originalAttackAnimLength / desiredDuration;
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
        attackTimer += Time.deltaTime;

        AttackOrChase();
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
                if (attackTimer >= 1f / attackSpeed)
                {
                    Debug.Log(gameObject.name + "Attack");
                    attackTimer = 0;
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

            }
            else
            {
                Move(_targetMonster.GameObject.transform.position);
            }

        }
    }

    public void Attack(IFighter target)
    {
        // 새로운 공격 명령이 들어오면 기존 대상을 갱신합니다.
        _targetMonster = target;
        // 대상의 위치로 이동을 시작합니다.
        // Update 메서드에서 거리를 체크하며 공격을 처리합니다.       
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
        // 이동 명령 시 공격 대상 초기화
        _myAnimator.SetFloat("attackSpeed", 0);
        agent.destination = position;
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
        _hp -= combatEvent.Damage;
        if (_hp <= 0)
        {
            //죽음
        }
    }
}