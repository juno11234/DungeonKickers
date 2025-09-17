using UnityEngine;

public class Player_Warrior : PlayerUnit
{
    public override void Skill()
    {
        guard = activeSO.value;
        //2 50퍼,4 75퍼 ,10 90퍼
    }
}
