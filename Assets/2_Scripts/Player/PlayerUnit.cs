using UnityEngine;
using UnityEngine.AI;

public abstract class PlayerUnit : MonoBehaviour, IFighter
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] protected PlayerDataSO playerSO;
    [SerializeField] private GameObject selectedMarker;
    private Collider _myCollider;
    public Collider MainCollider => _myCollider;
    public GameObject GameObject => gameObject;
    private int _hp;
    private float attackRange;
    private float attackSpeed;
    private float attackTimer;
    // 공격할 대상을 저장하는 변수 추가
    private IFighter _targetMonster;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        _myCollider = GetComponent<Collider>();

        selectedMarker.SetActive(false);

        agent.speed = playerSO.moveSpeed;
        _hp = playerSO.hp;
        attackRange = playerSO.attackRange;
        attackSpeed = playerSO.attackSpeed;
        attackTimer = playerSO.attackSpeed;
    }

    // Update 메서드 추가
    // 매 프레임마다 공격 상태를 확인합니다.
    private void Update()
    {
        attackTimer += Time.deltaTime;
        if (_targetMonster != null)
        {
            // 대상과의 거리 계산
            float distance = Vector3.Distance(transform.position, _targetMonster.GameObject.transform.position);

            // 사거리 안에 들어오면 공격 시작
            if (distance <= attackRange)
            {
                // 이동을 멈춥니다.
                agent.ResetPath();
                if (attackTimer >= attackSpeed)
                {
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

    public void Attack(IFighter monster)
    {
        // 새로운 공격 명령이 들어오면 기존 대상을 갱신합니다.
        _targetMonster = monster;

        // 대상의 위치로 이동을 시작합니다.
        // Update 메서드에서 거리를 체크하며 공격을 처리합니다.
        Move(monster.GameObject.transform.position);
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
        agent.destination = position;
    }
    public void OnMonsterDied()
    {
        // 몬스터가 죽었을 때 대상을 초기화하는 메서드
        if (_targetMonster != null && _targetMonster.GameObject == null)
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