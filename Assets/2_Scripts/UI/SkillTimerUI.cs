using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class SkillTimerUI : MonoBehaviour
{
    [SerializeField] private Image cooldownMask;
    [SerializeField] private TMP_Text cooldownText;
    private PlayerUnit playerUnit; // 쿨타임 정보를 받을 유닛
    private float skillCool;


    private static readonly string[] cachedStrings = new string[61];
    private int prevRemain = -1;
    public void Init(PlayerUnit unit)
    {
        playerUnit = unit;
        skillCool = playerUnit.GetSkillCool();

        if (cachedStrings[0] == null)
        {
            for (int i = 0; i < cachedStrings.Length; i++)
            {
                cachedStrings[i] = i.ToString();
            }
        }
    }

    private void Update()
    {
        // skillAble == false → 쿨타임 중
        if (playerUnit.SkillAble == false)
        {
            float remain = playerUnit.skillTimer;
            float fill = remain / skillCool;

            cooldownMask.fillAmount = fill;
            int remainSec = Mathf.CeilToInt(remain);

            if (remainSec != prevRemain)
            {
                prevRemain = remainSec;
                int index = Mathf.Clamp(remainSec, 0, cachedStrings.Length - 1);
                cooldownText.text = cachedStrings[index];
            }

            cooldownText.gameObject.SetActive(true);
        }
        else
        {
            cooldownMask.fillAmount = 0f;
            cooldownText.gameObject.SetActive(false);
            prevRemain = -1;
        }
    }
}
