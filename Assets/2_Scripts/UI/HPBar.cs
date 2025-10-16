using System;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Vector3 worldOffset = new Vector3(0, 2.5f, 0);

    private Camera mainCam;
    private RectTransform canvasRect;
    private RectTransform rect;
    private PlayerUnit target;

    public void Init(PlayerUnit unit, RectTransform canvas)
    {
        target = unit;
        canvasRect = canvas;
        rect = GetComponent<RectTransform>();
        mainCam = Camera.main;
        target.HpChanged += OnHpChanged;

        OnHpChanged(target.CurrentHp, target.MaxHp);
    }
    private void Update()
    {
        if (target == null || target.OnDie)
        {
            gameObject.SetActive(false);
            return;
        }

        Vector3 worldPos = target.transform.position + worldOffset;
        Vector3 screenPos = mainCam.WorldToScreenPoint(worldPos);

        if (screenPos.z < 0)
        {
            hpSlider.gameObject.SetActive(false);
            return;
        }
        else hpSlider.gameObject.SetActive(true);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, screenPos, mainCam, out Vector2 localPos);

        rect.localPosition = localPos;
    }

    private void OnHpChanged(int current, int max)
    {
        hpSlider.value = (float)current / max;
    }
}
