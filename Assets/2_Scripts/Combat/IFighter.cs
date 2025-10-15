using UnityEngine;

public interface IFighter
{
    public Collider MainCollider { get; }
    public GameObject GameObject { get; }
    public bool OnDie { get; }
    public void TakeDamage(CombatEvent combatEvent);
    public void TakeHeal(HealEvent healEvent);
    public void AttackEvent();
}
