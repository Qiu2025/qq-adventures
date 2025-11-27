using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // --------------------------------------------- //
    private static bool gameOver = false;
    private static bool slowmo = false;
    
    // --- PARA PUNTUACION POR LOS COLLECTIBLES --- //
    [HideInInspector] public static int score = 0;

    // -------- PARA RESPAWN EN CHECKPOINT -------- //
    private static Vector2 lastCheckpointPos;
    private static GameObject player;
    
    // -------- PARA BOCADILLO ------------------- //
    private static GameObject chat; 

    // -------- PARA CIRCLE FADE ------------------- //
    private GameObject UICanvas;
    private Animator UI_animator;
    private const float ANIMATION_DURATION = 1.5f;
    private float lastRespawnTime = 0;
    private Animator player_animator;
    private PlayerMovement player_script;
    private Rigidbody2D player_rb;

    // --------------------------------------------- //

    [Header("Selección de mecánicas")]
    public bool allowDoubleJump = false;
    public bool allowDash = false;
    public bool allowGlide = false;

    // --------------------------------------------- //
    private static Animator powerFxAnimator; 
    private static GameObject powerFx;
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);  // si ya había uno, este se destruye
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);  // este vive entre escenas

        player = GameObject.FindGameObjectWithTag("Player");
        player_animator = player.GetComponent<Animator>();
        player_script = player.GetComponent<PlayerMovement>();
        player_rb = player.GetComponent<Rigidbody2D>();

        lastCheckpointPos = player.transform.position;

        chat =  GameObject.FindGameObjectWithTag("Chat");
        UICanvas = GameObject.FindGameObjectWithTag("UICanvas");
        UI_animator = UICanvas.GetComponent<Animator>();
        
        powerFx = GameObject.FindGameObjectWithTag("PowerEffect");
        if (powerFx != null)
        {
            powerFxAnimator = powerFx.GetComponent<Animator>();
            powerFx.SetActive(false);
        }

        Application.targetFrameRate = 144;
        
    }

    void Update()
    {
        bool canRespawn = UI_animator.GetCurrentAnimatorStateInfo(0).IsName("Idle");

        // Para volver al ultimo checkpoint
        if (Input.GetKeyDown(KeyCode.R) && canRespawn)
        {
            lastRespawnTime = Time.time;
            StartCoroutine(TeleportWithTransition());
        }

        // Slow motion, para debug
        if (Input.GetKeyDown(KeyCode.T))
        {
            slowmo = !slowmo;
            Time.timeScale = slowmo ? 0.2f : 1f;
        }
    }
    
    // --------------------------------------------- //

    public static void SetCheckpoint(Vector2 position)
    {
        lastCheckpointPos = position;
        Debug.Log("Checkpoint guardado en: " + position);
    }

    public static void RespawnPlayer()
    {
        Collider2D col = player.GetComponent<BoxCollider2D>();
        col.enabled = false;

        player.transform.position = lastCheckpointPos;

        col.enabled = true;

        Debug.Log("Jugador reaparecido en checkpoint");
        gameOver = false;
        Time.timeScale = 1f;
    }

    // Corutina que realiza la transicion y hace respawn
    IEnumerator TeleportWithTransition()
    {
        // Prohibir el movimiento del jugador durante la transicion
        player_animator.Play("Player Turn");
        player_script.canMove = false;
        player_rb.linearVelocity = Vector2.zero;

        // 1. Fade in
        UI_animator.SetTrigger("Start");
        yield return new WaitForSeconds(ANIMATION_DURATION);

        // 3. Teleport
        RespawnPlayer();

        // 4. Fade out
        UI_animator.SetTrigger("End");
        yield return new WaitForSeconds(ANIMATION_DURATION);

        UI_animator.SetTrigger("BackToIdle");

        // Permitir el movimiento
        player_script.canMove = true;
        player_animator.Play("Player Idle");
    }

   // --------------------------------------------- //

    public static void SetGameOver(bool cond) {
        gameOver = cond;
        Time.timeScale = gameOver ? 0f: 1f;
    }
    
    // --------------------------------------------- //

    public static void ShowChat(Sprite sprite, float fadeIn, float hold, float fadeOut, 
                                float scale = 1f, float offsetX = 0f, float offsetY = 0f)
    {
        if (chat == null || sprite == null) return;

        Instance.StartCoroutine(Instance.ShowChatRoutine(sprite, fadeIn, hold, fadeOut, scale, offsetX, offsetY));
    }

    private IEnumerator ShowChatRoutine(Sprite sprite, float fadeIn, float hold, float fadeOut,
                                        float scale, float offsetX, float offsetY)
    {
        // Hijo que contiene el SpriteRenderer del chat
        SpriteRenderer imgRenderer = chat.transform.Find("ImagenMostrar").GetComponent<SpriteRenderer>();
        if (imgRenderer == null) yield break;

        // Asignar el sprite
        imgRenderer.sprite = sprite;

        // Ajustar escala
        imgRenderer.transform.localScale = Vector3.one * scale;

        // Ajustar posición relativa
        imgRenderer.transform.localPosition = new Vector3(offsetX, offsetY, imgRenderer.transform.localPosition.z);

        // Activar chat
        chat.SetActive(true);

        // Poner alfa 0 a todos los SpriteRenderers del chat
        SpriteRenderer[] renderers = chat.GetComponentsInChildren<SpriteRenderer>(true);
        foreach (var r in renderers)
        {
            Color c = r.color;
            c.a = 0f;
            r.color = c;
        }

        // Fade in
        float t = 0f;
        while (t < fadeIn)
        {
            t += Time.deltaTime;
            float a = t / fadeIn;
            foreach (var r in renderers)
            {
                Color c = r.color;
                c.a = a;
                r.color = c;
            }
            yield return null;
        }

        // Mantener visible
        foreach (var r in renderers)
        {
            Color c = r.color;
            c.a = 1f;
            r.color = c;
        }
        yield return new WaitForSeconds(hold);

        // Fade out
        t = 0f;
        while (t < fadeOut)
        {
            t += Time.deltaTime;
            float a = 1f - t / fadeOut;
            foreach (var r in renderers)
            {
                Color c = r.color;
                c.a = a;
                r.color = c;
            }
            yield return null;
        }

        // Ocultar chat
        chat.SetActive(false);
    }

   // --------------------------------------------- //

   public static void PowerEffect()
   {
       powerFx.SetActive(true);
       powerFxAnimator.Play("Power", 0, 0f);        // reproducir desde el frame 0
       Instance.StartCoroutine(DisableFxWhenDone());
   }
   private static IEnumerator DisableFxWhenDone()
   {
       // espera la duración de la animación
       yield return new WaitForSeconds(
           powerFxAnimator.GetCurrentAnimatorStateInfo(0).length
       );

       powerFx.SetActive(false);
   }
   
}
