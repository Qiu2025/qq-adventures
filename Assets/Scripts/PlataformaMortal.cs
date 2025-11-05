using UnityEngine;

public class PlataformaMortal : MonoBehaviour
{
    [SerializeField] string tagObjetivo = "Player";
    [SerializeField] string causa = "plataforma";

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag(tagObjetivo))
        {
            Debug.Log($"💀 Matado por {causa}");
            // Aquí tu lógica de muerte o respawn
        }
    }
}
