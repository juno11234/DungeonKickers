using UnityEngine;
using UnityEngine.AI;

public abstract class PlayerUnit : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] protected PlayerDataSO playerSO;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = playerSO.moveSpeed;
    }
    public void Attack()
    {

    }
    public void Move(Transform target)
    {
        agent.destination = target.position;
    }
}


