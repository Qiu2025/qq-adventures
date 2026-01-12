using UnityEngine;

public class DashCooldownBar : MonoBehaviour
{
    [SerializeField] private RectTransform fillRect;
    [SerializeField] private RectTransform bgRect;
    [SerializeField] private GameObject rootToHide;

    private float fullWidth;

    private void Awake()
    {
        if (bgRect == null)
        {
            fullWidth = 0f;
            return;
        }

        fullWidth = bgRect.rect.width;
    }

    public void SetReady01(float ready01)
    {
        if (fillRect == null) return;

        if (fullWidth <= 0f)
            fullWidth = bgRect != null ? bgRect.rect.width : fillRect.rect.width;

        ready01 = Mathf.Clamp01(ready01);
        float percent = 1f - ready01;

        var size = fillRect.sizeDelta;
        size.x = fullWidth * percent;
        fillRect.sizeDelta = size;

        if (rootToHide) rootToHide.SetActive(ready01 < 1f);
    }

    public void OnDashUsed() => SetReady01(0f);
    public void OnReady() => SetReady01(1f);
}