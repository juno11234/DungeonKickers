using UnityEngine;
using UnityEngine.AI;

public abstract class PlayerUnit : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] protected PlayerDataSO playerSO;
    [SerializeField] private GameObject selectedMarker;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = playerSO.moveSpeed;
        selectedMarker.SetActive(false);
    }

    public void Attack()
    {
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
        agent.destination = position;
    }
}