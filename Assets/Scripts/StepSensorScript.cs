using UnityEngine;

// Script para realizar la caida de las plataformas
public class PlataformaStepSensor : MonoBehaviour
{
    private PlataformaTemporal plataforma;

    void Awake()
    {
        plataforma = GetComponentInParent<PlataformaTemporal>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Llama a la plataforma para iniciar la caída (si no ha empezado)
            plataforma.ActivarPorPisada(other);
        }
    }
}
