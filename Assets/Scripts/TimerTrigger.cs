using UnityEngine;

public class TimerTrigger : MonoBehaviour
{
    [SerializeField] private GameObject temporizador;
    [SerializeField] private GameObject tiempoUI;
    [SerializeField] private BoxCollider2D triggerCollider;

    void OnTriggerExit2D(Collider2D collision)
    {
        // Comprobando si el jugador realizó la salida por la derecha
        if (collision.CompareTag("Player") && !PlayerMovement.getIsFacingLeft())
        {
            temporizador.SetActive(!temporizador.activeSelf);
            tiempoUI.SetActive(!tiempoUI.activeSelf);
            triggerCollider.isTrigger = false;
        }
    }
    
}
