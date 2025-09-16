using System;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour, IFighter
{
    public Collider MainCollider => _collider;
    public GameObject GameObject => gameObject;
    private Collider _collider;

    private void Start()
    {
        _collider = GetComponent<Collider>();
        CombatSystem.Instance.RegisterMonster(this);
        Debug.Log(this.gameObject.name);
    }

    public void TakeDamage(CombatEvent combatEvent)
    {
    }
}