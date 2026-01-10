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
    
    //  Variables añadidas
    private GameObject activeObject;
    private Coroutine currentRoutine;

    void Update()
    {
        if (isPlayerNearby && !isPlaying)
        {
            if (Input.GetKeyDown(KeyCode.E))
                currentRoutine = StartCoroutine(PlayGhostRun());
            else if (Input.GetKeyDown(KeyCode.Return))
                currentRoutine = StartCoroutine(PlayPlayerAutoPilot());
        }

        //  Detección de cancelación
        if (isPlaying && DetectInput())
        {
            CancelAll();
        }
    }

    //Métodos de control y seguridad
    bool DetectInput()
    {
        return Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || 
               Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D) || 
               Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.1f;
    }

    void CancelAll()
    {
        if (currentRoutine != null) StopCoroutine(currentRoutine);
        GhostRunner.Instance.StopReplayManual();

        if (activeObject != null && !activeObject.CompareTag("Player"))
            Destroy(activeObject);

        RestorePlayerPhysics();
        
        ghostCamera.Priority = 5;
        ghostCamera.Follow = null;
        
        // CORRECCIÓN: Solo muestra prompt si sigue cerca
        floatingPrompt.SetActive(isPlayerNearby);
        
        isPlaying = false;
        if(animator) animator.SetTrigger("isFinished");
    }

    IEnumerator PlayGhostRun()
    {
        if(animator) animator.SetTrigger("isPressed");
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
        activeObject = GhostRunner.Instance.PlayRunByName(runName);  // Obtener el ghost
        
        if (activeObject != null)
        {
            ghostCamera.Follow = activeObject.transform;    // Segunda camara -> ghost instanciado

            // Estas cuatro lineas son para arreglar el problema de tp que ocurre raramente
            yield return null;
            ghostCamera.transform.position = activeObject.transform.position;
            ghostCamera.PreviousStateIsValid = false;
            yield return null;

            ghostCamera.Priority = 20; // Cambiar de camara
            
            // Duración real
            float duration = GhostRunner.Instance.GetRunDuration(runName);
            yield return new WaitForSeconds(duration);
            
            ghostCamera.Priority = 5; // Volver a la camara del player
            ghostCamera.Follow = null;  // Limpiar segunda camara
            
            Destroy(activeObject);
        }

        // Devolver el control tras tener terminar el blend camara ghost -> camara original
        yield return new WaitForSeconds(1f);
        playerScript.canMove = true;
        playerRb.simulated = true;
        
        playerAnimator.Play("Player Idle");
        
        // CORRECCIÓN: Solo muestra prompt si sigue cerca
        floatingPrompt.SetActive(isPlayerNearby);
        
        isPlaying = false;
        if(animator) animator.SetTrigger("isFinished");
    }

    // Corrutina Autopiloto
    IEnumerator PlayPlayerAutoPilot()
    {
        if(animator) animator.SetTrigger("isPressed");
        isPlaying = true;
        floatingPrompt.SetActive(false);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        activeObject = player;

        var pm = player.GetComponent<PlayerMovement>();
        var rb = player.GetComponent<Rigidbody2D>();
        var playerAnim = player.GetComponent<Animator>();

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
            if (playerAnim != null)
            {
                bool animDiceDerecha = playerAnim.GetBool("isFacingRight");
                pm.isFacingRight = !animDiceDerecha; 
            }
            timer += Time.deltaTime;
            yield return null;
        }

        RestorePlayerPhysics();

        ghostCamera.Priority = 5;
        ghostCamera.Follow = null;
        
        floatingPrompt.SetActive(isPlayerNearby);
        isPlaying = false;
        if(animator) animator.SetTrigger("isFinished");
    }

    // Método de restauración
    void RestorePlayerPhysics()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            var rb = player.GetComponent<Rigidbody2D>();
            var pm = player.GetComponent<PlayerMovement>();
            var playerAnim = player.GetComponent<Animator>();

            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.simulated = true;
            pm.canMove = true;
            
            if (playerAnim != null)
            {
                bool finalAnimValue = playerAnim.GetBool("isFacingRight");
                pm.isFacingRight = !finalAnimValue; 
                playerAnim.Play("Player Idle");
            }
        }
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