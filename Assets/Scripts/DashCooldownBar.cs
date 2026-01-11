using UnityEngine;

public class DashCooldownBar : MonoBehaviour
{
    [SerializeField] private RectTransform fillRect;   // BarFill
    [SerializeField] private RectTransform bgRect;     // BarBG
    [SerializeField] private GameObject rootToHide;

    private float fullWidth;

    private void Awake()
    {
        // Usar el ancho del background como ancho de la barra
        fullWidth = bgRect.rect.width;
    }
    
    public void SetReady01(float ready01)
    {
        ready01 = Mathf.Clamp01(ready01);
        float percent = 1f - ready01;

        // Efecto desde el centro: cambia el ancho, el pivote está centrado así que crece hacia ambos lados
        var size = fillRect.sizeDelta;
        size.x = fullWidth * percent;
        fillRect.sizeDelta = size;

        // Ocultar cuando se acaba el cooldown
        if (rootToHide) rootToHide.SetActive(ready01 < 1f);
    }

    public void OnDashUsed() => SetReady01(0f);
    public void OnReady()    => SetReady01(1f);
}