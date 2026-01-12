using UnityEngine;

public class ChatTrigger : MonoBehaviour
{
    [Header("Referencia Externa")]
    public Door puerta;

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
        // SI LA PUERTA YA SE ABRIÓ, NO HACEMOS NADA
        if (puerta != null && puerta.abierta) return;

        // Verificamos si lo que entró en la zona es el Jugador
        if (collision.CompareTag("Player") && !activado)
        {
            // Llamamos a la función con TODOS tus parámetros originales
            GameManager.ShowChat(imagenAMostrar, tiempoAparicion, tiempoEspera, tiempoDesaparicion, escala, desplazamientoX, desplazamientoY);

            if (soloUnaVez)
            {
                activado = true;
            }
        }
    }
}