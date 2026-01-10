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
    private bool isPlaying = false;
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

        if (isPlaying && DetectInput())
        {
            CancelAll();
        }
    }

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
        floatingPrompt.SetActive(true);
        isPlaying = false;
        if(animator) animator.SetTrigger("isFinished");
    }

    IEnumerator PlayGhostRun()
    {
        if(animator) animator.SetTrigger("isPressed");
        isPlaying = true;
        floatingPrompt.SetActive(false);

        activeObject = GhostRunner.Instance.PlayRunByName(runName);
        if (activeObject != null)
        {
            ghostCamera.Follow = activeObject.transform;
            ghostCamera.Priority = 20;
            float duration = GhostRunner.Instance.GetRunDuration(runName);
            yield return new WaitForSeconds(duration);
            
            ghostCamera.Priority = 5;
            ghostCamera.Follow = null;
            Destroy(activeObject);
        }

        floatingPrompt.SetActive(true);
        isPlaying = false;
        if(animator) animator.SetTrigger("isFinished");
    }

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

        // 1. Bloquear control(Antes, ahora ya no)
        //LO he modificado para que funcione más fluido, si hay bugs
        // hay que ponerlo a false
        pm.canMove = true;
        rb.linearVelocity = Vector2.zero;

        // 2. Iniciar Grabación
        GhostRunner.Instance.PlayRunOnExistingTarget(runName, player);

        ghostCamera.Follow = player.transform;
        ghostCamera.Priority = 20;

        float duration = GhostRunner.Instance.GetRunDuration(runName);
        float timer = 0f;

        // 3. BUCLE FRAME A FRAME CON LOGICA INVERTIDA
        while (timer < duration)
        {
            if (playerAnim != null)
            {
                // Leemos lo que dice la grabación
                bool animDiceDerecha = playerAnim.GetBool("isFacingRight");
                
                // APLICAMOS LA LÓGICA INVERSA QUE PEDISTE:
                // Si anim es true, isFacingRight será false.
                // Si anim es false, isFacingRight será true.
                pm.isFacingRight = !animDiceDerecha; 
            }
            
            timer += Time.deltaTime;
            yield return null;
        }

        // 4. Salida
        RestorePlayerPhysics();

        ghostCamera.Priority = 5;
        ghostCamera.Follow = null;
        
        floatingPrompt.SetActive(true);
        isPlaying = false;
        if(animator) animator.SetTrigger("isFinished");
    }

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
            
            // Aseguramos el último frame con la misma lógica invertida
            if (playerAnim != null)
            {
                bool finalAnimValue = playerAnim.GetBool("isFacingRight");
                pm.isFacingRight = !finalAnimValue; // Lógica inversa aquí también
                
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