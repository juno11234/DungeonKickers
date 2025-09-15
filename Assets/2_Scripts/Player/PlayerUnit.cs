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
    public void Move(Vector3 position)
    {
        agent.destination = position;
    }
}


