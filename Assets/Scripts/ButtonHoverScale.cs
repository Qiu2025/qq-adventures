using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverScale : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler
{
    public Vector3 scaleHover = new Vector3(1.3f, 1.3f, 1.3f);
    private Vector3 scaleOriginal;

    void Start()
    {
        scaleOriginal = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = scaleHover;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = scaleOriginal;
    }
}
