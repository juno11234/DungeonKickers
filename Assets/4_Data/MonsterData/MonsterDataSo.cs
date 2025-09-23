using UnityEngine;

[CreateAssetMenu(fileName = "MonsterData", menuName = "Data/MonsterData")]
public class MonsterDataSo : UnitDataSO
{
    public int hp;
    public int attackDamage;
    public float attackSpeed;
    public int moveSpeed;
    public float attackRange;
    public float EXP;

    public override UnitStats GetUnitStats()
    {
        return new UnitStats(hp, attackRange, attackSpeed, 0, moveSpeed);
    }
}
