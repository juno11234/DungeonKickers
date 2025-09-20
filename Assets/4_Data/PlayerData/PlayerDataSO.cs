using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Data/PlayerData")]
public class PlayerDataSO : BaseDataSO
{
    public string job;
    public int level;
    public int exp;
    public int hp;
    public int attackDamage;
    public int defence;
    public int moveSpeed;
    public float attackSpeed;
    public int mana;
    public float attackRange;
}
