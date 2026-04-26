using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EXPManager : MonoBehaviour
{
    [SerializeField] private TMP_Text expText;
    [SerializeField] private Slider EXPSlider;
    

    public Action OnLevelUp;
    private int currentLevel = 1;
    private float currentExp = 0f;
    private float levelUpExp = 100f;
    private int skillPoint = 0;

    public int CurrentLevel => currentLevel;
    public float CurrentExp => currentExp;
    public float LevelUpExp => levelUpExp;
    public int CurrentSkillPoint => skillPoint;
    private void Start()
    {
        expText.text += currentLevel;
        EXPSlider.value = currentExp / levelUpExp;
    }

    public void AddExp(float amount)
    {
        currentExp += amount;

        if (currentExp >= levelUpExp)
        {
            LevelUp();
        }

        EXPSlider.value = currentExp / levelUpExp;
    }
    private void LevelUp()
    {
        skillPoint++;
        currentLevel++;
        currentExp -= levelUpExp;
        levelUpExp = GetExpRequired(currentLevel);

        string level = $"{currentLevel}";
        expText.text = level;

        OnLevelUp?.Invoke(); // 다른 시스템(이펙트, UI 등)에 알림
        if (currentExp >= levelUpExp)
        {
            LevelUp();
        }
    }
    private float GetExpRequired(int level)
    {
        return Mathf.Pow(level, 2) * 50f;
    }

}
