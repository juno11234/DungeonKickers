using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Player_Mage : TargetingSkillerBase
{
    [SerializeField] private FireBall fireBallPrefab;
    [SerializeField] private Transform firePos;
    [SerializeField] private Texture2D skillCursor;

    public override void Skill()
    {
        base.Skill();
        RangeCheck();
    }

    public override void CursorChange()
    {
        InvokeCursorChange(skillCursor);
    }

    protected override void CastSkill()
    {
        FireBall fireBall = Instantiate(fireBallPrefab, firePos.position, Quaternion.identity);
        fireBall.Init(TargetPos);
    }
}
