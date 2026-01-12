using UnityEngine;
using System.Collections;
using Cinemachine;

public class GhostTrigger : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private string runName;
    [SerializeField] private GameObject floatingPrompt;

    [Header("Referencias")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private CinemachineVirtualCamera ghostCamera;
    [SerializeField] private Animator animator;

    private bool isPlayerNearby = false;
    private bool isPlayingGhost = false;
    private bool isPlayingPlayer = false;
    private GameObject ghostInstance;
    private Rigidbody2D rb;
    private PlayerMovement pm;
    private Animator playerAnim;
    private GameObject player;
    private Coroutine pilotInstance;

    private bool isAutoPilotActivated = false;
    private bool subscribed = false;

    // ----------------------------------------------------------------------

    void Start()
    {
            player = GameObject.FindGameObjectWithTag("Player");
            rb = player.GetComponent<Rigidbody2D>();
            pm = player.GetComponent<PlayerMovement>();
            playerAnim = player.GetComponent<Animator>();
    }

    void Update()
    {
        if (isPlayerNearby && !isPlayingGhost && !isPlayingPlayer)
        {
            if (Input.GetKeyDown(KeyCode.E))
                StartCoroutine(PlayGhostRun());
            else if (Input.GetKeyDown(KeyCode.Return) && isAutoPilotActivated)
                pilotInstance = StartCoroutine(PlayPlayerAutoPilot());
        }

        // Detección de cancelación
        if (isPlayingPlayer && DetectInput())
        {
            StopCoroutine(pilotInstance);
            GhostRunner.Instance.StopReplayManual();
            Destroy(ghostInstance);

            RestorePlayerPhysics();
            
            ghostCamera.Priority = 5;
            ghostCamera.Follow = null;
            floatingPrompt.SetActive(isPlayerNearby);
            isPlayingPlayer = false;
            animator.SetTrigger("isFinished");
        }
    }

    // ----------------------------------------------------------------------

    bool DetectInput()
    {
        return Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || 
               Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D) || 
               Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.1f;
    }

    void RestorePlayerPhysics()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.simulated = true;
        pm.canMove = true;
        
        bool finalAnimValue = playerAnim.GetBool("isFacingRight");
        pm.isFacingRight = !finalAnimValue; 
        playerAnim.Play("Player Idle");
    }

    // ----------------------------------------------------------------------

    IEnumerator PlayGhostRun()
    {
        if(animator) animator.SetTrigger("isPressed");
        isPlayingGhost = true;
        floatingPrompt.SetActive(false);

        // Bloquear jugador
        pm.canMove = false;
        rb.simulated = false;
        rb.linearVelocity = Vector2.zero;
        playerAnim.Play("Player Turn");

        // Logica de cambio de camaras
        ghostInstance = GhostRunner.Instance.PlayRunByName(runName);  // Obtener el ghost
        
        if (ghostInstance != null)
        {
            ghostCamera.Follow = ghostInstance.transform;    // Segunda camara -> ghost instanciado

            // Estas cuatro lineas son para arreglar el problema de tp que ocurre raramente
            yield return null;
            ghostCamera.transform.position = ghostInstance.transform.position;
            ghostCamera.PreviousStateIsValid = false;
            yield return null;

            ghostCamera.Priority = 20; // Cambiar de camara
            yield return new WaitForSeconds(GhostRunner.Instance.GetRunDuration(runName));
            ghostCamera.Priority = 5; // Volver a la camara del player
            ghostCamera.Follow = null;  // Limpiar segunda camara
            
            Destroy(ghostInstance);
        }

        // Devolver el control tras tener terminar el blend camara ghost -> camara original
        yield return new WaitForSeconds(2f);
        pm.canMove = true;
        rb.simulated = true;
        
        playerAnim.Play("Player Idle");
        floatingPrompt.SetActive(isPlayerNearby);
        isPlayingGhost = false;
        animator.SetTrigger("isFinished");
    }

    // Corrutina Autopiloto
    IEnumerator PlayPlayerAutoPilot()
    {
        animator.SetTrigger("isPressed");
        isPlayingPlayer = true;
        floatingPrompt.SetActive(false);

        // Lógica para que no se bloquee internamente
        pm.canMove = true;
        rb.linearVelocity = Vector2.zero;

        GhostRunner.Instance.PlayRunOnExistingTarget(runName, player);
        ghostCamera.Follow = player.transform;
        ghostCamera.Priority = 20;

        float duration = GhostRunner.Instance.GetRunDuration(runName);
        float timer = 0f;
        while (timer < duration)
        {
            bool animDiceDerecha = playerAnim.GetBool("isFacingRight");
            pm.isFacingRight = !animDiceDerecha; 
            timer += Time.deltaTime;
            yield return null;
        }

        RestorePlayerPhysics();

        ghostCamera.Priority = 5;
        ghostCamera.Follow = null;
        floatingPrompt.SetActive(isPlayerNearby);
        isPlayingPlayer = false;
        animator.SetTrigger("isFinished");
    }

    // ----------------------------------------------------------------------

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = true;
            if(!isPlayingGhost && !isPlayingPlayer) floatingPrompt.SetActive(true);
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

    void Apply()
    {
        var acc = AccessibilityManager.Instance;
        if (acc == null) return;

        Debug.Log("PLAYER recibió OnChangedAutopilot y aplica settings");
        Debug.Log(isAutoPilotActivated);
        isAutoPilotActivated = !isAutoPilotActivated;
        Debug.Log(isAutoPilotActivated);
    }

    private void OnEnable()
    {
        StartCoroutine(SubscribeWhenReady());
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    IEnumerator SubscribeWhenReady()
    {
        while (AccessibilityManager.Instance == null)
            yield return null;

        if (!subscribed)
        {
            AccessibilityManager.Instance.OnChangedAutopilot += Apply;
            subscribed = true;
        }
    }

    void Unsubscribe()
    {
        if (AccessibilityManager.Instance != null && subscribed)
        {
            AccessibilityManager.Instance.OnChangedAutopilot -= Apply;
            subscribed = false;
        }
    }
}