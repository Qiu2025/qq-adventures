using UnityEngine;
using System.Collections;
using Cinemachine;

public class GhostTrigger : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private string runName;
    [SerializeField] private float runDuration = 5f;
    [SerializeField] private GameObject floatingPrompt;

    [Header("Referencias")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private CinemachineVirtualCamera ghostCamera;
    [SerializeField] private Animator animator;

    private bool isPlayerNearby = false;
    private bool isPlaying = false;

    void Update()
    {
        if (isPlayerNearby && !isPlaying && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(PlayGhostRun());
        }
    }

    IEnumerator PlayGhostRun()
    {
        animator.SetTrigger("isPressed");
        isPlaying = true;
        floatingPrompt.SetActive(false);

        // Bloquear jugador
        PlayerMovement playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        Rigidbody2D playerRb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
        Animator playerAnimator = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
        playerScript.canMove = false;
        playerRb.simulated = false;
        playerRb.linearVelocity = Vector2.zero;
        playerAnimator.Play("Player Turn");

        // Logica de cambio de camaras
        GameObject spawnedGhost = GhostRunner.Instance.PlayRunByName(runName);  // Obtener el ghost
        ghostCamera.Follow = spawnedGhost.transform;    // Segunda camara -> ghost instanciado

        // Estas cuatro lineas son para arreglar el problema de tp que ocurre raramente
        yield return null;
        ghostCamera.transform.position = spawnedGhost.transform.position;
        ghostCamera.PreviousStateIsValid = false;
        yield return null;

        ghostCamera.Priority = 20; // Cambiar de camara
        yield return new WaitForSeconds(runDuration);
        ghostCamera.Priority = 5; // Volver a la camara del player
        ghostCamera.Follow = null;  // Limpiar segunda camara

        // Devolver el control tras tener terminar el blend camara ghost -> camara original
        yield return new WaitForSeconds(2f);
        playerScript.canMove = true;
        playerRb.simulated = true;
        
        playerAnimator.Play("Player Idle");
        floatingPrompt.SetActive(true);
        isPlaying = false;
        animator.SetTrigger("isFinished");
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = true;
            if(!isPlaying) floatingPrompt.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = false;
            floatingPrompt.SetActive(false);
        }
    }
}