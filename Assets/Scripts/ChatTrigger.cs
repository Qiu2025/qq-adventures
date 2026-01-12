using UnityEngine;

public class ChatTrigger : MonoBehaviour
{
    [Header("Configuración de la Imagen")]
    public Sprite imagenAMostrar;
    public float tiempoAparicion = 0.5f;
    public float tiempoEspera = 2.0f;
    public float tiempoDesaparicion = 0.5f;

    [Header("Ajustes Visuales")]
    public float escala = 1f;
    public float desplazamientoX = 0f;
    public float desplazamientoY = 1.5f;

    [Header("Opciones")]
    public bool soloUnaVez = true;
    private bool activado = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verificamos si lo que entró en la zona es el Jugador
        if (collision.CompareTag("Player") && !activado)
        {
            // Llamamos a la función estática de GameManager
            GameManager.ShowChat(imagenAMostrar, tiempoAparicion, tiempoEspera, tiempoDesaparicion, escala, desplazamientoX, desplazamientoY);

            if (soloUnaVez)
            {
                activado = true;
                
            }
        }
    }
}