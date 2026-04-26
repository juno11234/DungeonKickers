using UnityEngine;

public class DebugManager : MonoBehaviour
{
    public float testExp = 10000;
    public void GETEXP()
    {
        EXPEvnet e = new()
        {
            Exp = testExp
        };
        CombatSystem.Instance.AddInGameEvent(e);
    }
}
