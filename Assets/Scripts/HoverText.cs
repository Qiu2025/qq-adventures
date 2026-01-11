using UnityEngine;
using UnityEngine.EventSystems;

public class HoverText : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject tooltip;

    void Start()
    {
        tooltip.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltip.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.SetActive(false);
    }
}
