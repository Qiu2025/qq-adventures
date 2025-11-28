using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuArrow : MonoBehaviour
{
    [Header("UI")]
    public RectTransform arrow; 

    [Header("Configuración")]
    public bool aparecerEnLaDerecha = false; // Marcar si quieres: "Botón <"
    public float padding = 20f; // Separación entre el botón y la flecha
    public float moveSpeed = 15f; // Velocidad del movimiento

    void Update()
    {
        // 1. Detectar qué botón está seleccionado actualmente
        GameObject selectedObj = EventSystem.current.currentSelectedGameObject;

        if (selectedObj != null && selectedObj.activeInHierarchy)
        {
            // Intentamos obtener el RectTransform del objeto seleccionado
            RectTransform target = selectedObj.GetComponent<RectTransform>();
            if (target != null)
            {
                MoveArrow(target);
            }
        }
    }

    void MoveArrow(RectTransform target)
    {
        // --- CALCULAR POSICIÓN Y (Altura) ---
        // Interpolación suave (Lerp) para que la flecha se deslice verticalmente
        Vector3 newPos = arrow.position;
        newPos.y = Mathf.Lerp(newPos.y, target.position.y, Time.unscaledDeltaTime * moveSpeed);

        // --- CALCULAR POSICIÓN X (Horizontal) ---
        float targetHalfWidth = target.rect.width / 2;
        
        if (aparecerEnLaDerecha)
        {
            // Posición: Centro del botón + mitad del ancho + separación
            newPos.x = target.position.x + targetHalfWidth + padding;
        }
        else
        {
            // Posición: Centro del botón - mitad del ancho - separación
            newPos.x = target.position.x - targetHalfWidth - padding;
        }

        // Aplicar la nueva posición
        arrow.position = newPos;
    }
}