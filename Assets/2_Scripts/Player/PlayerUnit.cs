using UnityEngine;
using UnityEngine.AI;

public abstract class PlayerUnit
{
    [SerializeField] NavMeshAgent agent;
    public void Attack()
    {
        
    }
    public void Move(Transform target)
    {

        agent.destination = target.position;
    }
}


