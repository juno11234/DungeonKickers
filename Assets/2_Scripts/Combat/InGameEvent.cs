using UnityEngine;

public abstract class InGameEvent
{
    public enum EventType
    {
        Unknown,
        Combat,
        Heal
    }

    public IFighter Sender { get; set; }
    public IFighter Receiver { get; set; }
    public abstract EventType Type { get; }
}

public class CombatEvent : InGameEvent
{
    public int Damage { get; set; }
    public Collider Collider { get; set; }
    public override EventType Type => EventType.Combat;
}
public class HealEvent : InGameEvent
{
    public override EventType Type => EventType.Heal;
    public int Heal { get; set; }
}