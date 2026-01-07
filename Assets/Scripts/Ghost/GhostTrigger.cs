using UnityEngine;
using System.Collections;

public class GhostTrigger : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private string runName;
    [SerializeField] private float runDuration = 5f;
    [SerializeField] private GameObject floatingPrompt;

    [Header("Referencias")]
    [SerializeField] private Transform ghostTransform;
    // [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private Animator animator;

    private bool isPlayerNearby = false;
    private bool isPlaying = false;

    void Update()
    {
        if (isPlayerNearby && !isPlaying && Input.GetKeyDown(KeyCode.E))
        {
            animator.SetTrigger("isPressed");
            StartCoroutine(PlayGhostSequence());
        }
    }

    IEnumerator PlayGhostSequence()
    {
        isPlaying = true;
        floatingPrompt.SetActive(false);

        // Hacer que el jugador no se pueda mover
        PlayerMovement playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        Rigidbody2D playerRb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
        playerScript.canMove = false;
        playerRb.simulated = false;
        playerRb.linearVelocity = Vector2.zero;

        // Cambio de follow de la camara
        // Transform originalCameraTarget = virtualCamera.Follow;
        // virtualCamera.Follow = ghostTransform;

        GhostRunner.Instance.PlayRunByName(runName);
        
        yield return new WaitForSeconds(runDuration);

        // virtualCamera.Follow = originalCameraTarget;

        yield return new WaitForSeconds(1f);

        // Devolver control
        playerScript.canMove = true;
        playerRb.simulated = true;
        
        if(isPlayerNearby) floatingPrompt.SetActive(true);
        
        isPlaying = false;
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