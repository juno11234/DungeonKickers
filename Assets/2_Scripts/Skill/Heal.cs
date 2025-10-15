using System.Collections;
using UnityEngine;

public class Heal : MonoBehaviour
{
    private PlayerUnit priest;
    private PlayerUnit targetPlayer;
    private int healTickAmount;
    private int healMaxCount;
    int count = 0;
    public void Init(int healTick, int healCount, PlayerUnit sender, PlayerUnit reciever)
    {
        priest = sender;
        targetPlayer = reciever;
        healMaxCount = healCount;
        healTickAmount = healTick;
        count = 0;

        StartCoroutine(HealingTick());
    }

    private void healing()
    {
        HealEvent healEvent = new()
        {
            Sender = priest,
            Receiver = targetPlayer,
            Heal = healTickAmount,
        };
        CombatSystem.Instance.AddInGameEvent(healEvent);
    }
    IEnumerator HealingTick()
    {
        while (count < healMaxCount)
        {
            healing();
            count++;
            yield return new WaitForSeconds(1f);
        }

        Destroy(gameObject);
    }
}
