using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuArrow : MonoBehaviour
{
    [Header("UI")]
    public RectTransform arrow; 

    [Header("Configuración")]
    public bool aparecerEnLaDerecha = false; 
    public float padding = 20f; 
    public float moveSpeed = 15f; 

    void Update()
    {
        GameObject selectedObj = EventSystem.current.currentSelectedGameObject;

        if (selectedObj != null && selectedObj.activeInHierarchy)
        {
            RectTransform target = selectedObj.GetComponent<RectTransform>();
            if (target != null)
            {
                MoveArrow(target);
            }
        }
    }

    void MoveArrow(RectTransform target)
    {
        Vector3 newPos = arrow.position;
        newPos.y = Mathf.Lerp(newPos.y, target.position.y, Time.unscaledDeltaTime * moveSpeed);
        float targetHalfWidth = target.rect.width / 2;
        
        if (aparecerEnLaDerecha)
        {
            newPos.x = target.position.x + targetHalfWidth + padding;
        }
        else
        {
            newPos.x = target.position.x - targetHalfWidth - padding;
        }
        
        arrow.position = newPos;
    }
}