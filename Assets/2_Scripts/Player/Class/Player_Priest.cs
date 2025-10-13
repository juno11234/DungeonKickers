using UnityEngine;

public class Player_Priest : TargetingSkillerBase
{
    [SerializeField] private FireBall fireBallPrefab;
    [SerializeField] private Texture2D skillCursor;

    public override void Skill()
    {
        //팀원에게 50 보호막
        base.Skill();
    }
    public override void CursorChange()
    {
         InvokeCursorChange(skillCursor);
    }
    protected override void CastSkill()
    {

    }
}
