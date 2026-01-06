using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private Animator animator;
    private bool activated = false;

    [SerializeField] private GameObject canvasTextoFlotante;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!activated && collision.CompareTag("Player"))
        {
            activated = true;
            animator.SetTrigger("Activate");
            GameManager.SetCheckpoint(transform.position);
            MostrarTiempo();
        }
    }

    void MostrarTiempo()
    {
        Vector3 posicion = transform.position + Vector3.up * 1.5f;
        GameObject canvasTexto = Instantiate(canvasTextoFlotante, posicion, Quaternion.identity);

        var scriptTexto = canvasTexto.GetComponentInChildren<TextoFlotante>();
        float tiempoActual = Time.timeSinceLevelLoad;
        scriptTexto.ConfigurarTexto("  " + tiempoActual.ToString("F2") + "s");
    }
}