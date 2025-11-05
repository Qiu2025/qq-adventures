using UnityEngine;

public class BolaPinchos : MonoBehaviour
{
    public float velocidad = 5f;
    private Vector2 direccion;

    public void Disparar(Vector2 dir)
    {
        direccion = dir.normalized;
        gameObject.SetActive(true);
    }

    void Update()
    {
        transform.Translate(direccion * velocidad * Time.deltaTime);
    }

    private void OnBecameInvisible()
    {
        // Cuando sale de la cámara, se desactiva para volver al pool
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Si toca al jugador
        if (collision.collider.CompareTag("Player"))
        {
            Debug.Log("💀 Matado por disparo del cactus");
            // Aquí podrías añadir lógica de daño o muerte del jugador
            gameObject.SetActive(false); // También puedes desactivarla si quieres que desaparezca
        }
    
        
            gameObject.SetActive(false);
        
    }
}
