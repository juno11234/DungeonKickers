using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Player_Mage : TargetingSkillerBase
{
    [SerializeField] private FireBall fireBallPrefab;
    [SerializeField] private Transform firePos;
    [SerializeField] private Texture2D skillCursor;

    private float fireBallRadius = 5f;

    public override void CursorChange()
    {
        InvokeCursorChange(skillCursor, 2 * fireBallRadius, true);
    }

    protected override void CastSkill()
    {
        Skill();
        FireBall fireBall = Instantiate(fireBallPrefab, firePos.position, Quaternion.identity);
        fireBall.Init(this, activeSkillSO.value, TargetPos, fireBallRadius);
    }
}
