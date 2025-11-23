using UnityEngine;

public class TimerTrigger : MonoBehaviour
{
    [SerializeField] private GameObject temporizador;
    [SerializeField] private GameObject tiempoUI;
    [SerializeField] private BoxCollider2D triggerCollider;
    private GameObject player;
    private PlayerMovement player_script;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        player_script = player.GetComponent<PlayerMovement>();
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        // Comprobando si el jugador realizó la salida por la derecha
        if (collision.CompareTag("Player") && player_script.isFacingRight)
        {
            temporizador.SetActive(!temporizador.activeSelf);
            tiempoUI.SetActive(!tiempoUI.activeSelf);
            triggerCollider.isTrigger = false;
        }
    }
    
}
