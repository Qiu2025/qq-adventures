using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; 
using System.Collections.Generic;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // --------------------------------------------- //
    private static bool gameOver = false;
    private static bool slowmo = false;
    private static bool isSlowMoActivated = false;

    [HideInInspector] public static int score = 0;
    // -------- PARA GUARDAR TIEMPOS POR ZONA -------- //
    public static Dictionary<string, float> zoneTimes =
        new Dictionary<string, float>();

    private static Vector2 lastCheckpointPos;
    private static GameObject player;
    
    private static GameObject chat; 

    // -------- PARA CIRCLE FADE ------------------- //
    private GameObject UICanvas;
    private Animator UI_animator;
    private const float ANIMATION_DURATION = 1.5f;
    private float lastRespawnTime = 0;
    
    // Variables del jugador (instancia)
    private Animator player_animator;
    private PlayerMovement player_script;
    private Rigidbody2D player_rb;
    private SpriteRenderer player_sr;

    [Header("Selección de mecánicas")]
    public bool allowDoubleJump = false;
    public bool allowDash = false;

    private static Animator powerFxAnimator; 
    private static GameObject powerFx;
    
    

    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        RefreshReferences();

        Application.targetFrameRate = 144;
        powerFx.SetActive(true);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        if (AccessibilityManager.Instance != null)
            AccessibilityManager.Instance.OnChangedSlowMo += Apply;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (AccessibilityManager.Instance != null)
            AccessibilityManager.Instance.OnChangedSlowMo -= Apply;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RefreshReferences();
        
        
        /* voy a guardar el score para cada escena, necesario para enseñarlo en la escena final, cambiar si no os convence 
        score = 0;
        */
    }

    void RefreshReferences()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        
        if (player != null)
        {
            player_animator = player.GetComponent<Animator>();
            player_script = player.GetComponent<PlayerMovement>();
            player_rb = player.GetComponent<Rigidbody2D>();
            player_sr = player.GetComponent<SpriteRenderer>();
            
            lastCheckpointPos = player.transform.position; 
        }

        chat = GameObject.FindGameObjectWithTag("Chat");
        
        UICanvas = GameObject.FindGameObjectWithTag("UICanvas");
        if (UICanvas != null)
        {
            UI_animator = UICanvas.GetComponent<Animator>();
        }
        
        powerFx = GameObject.FindGameObjectWithTag("PowerEffect");
        if (powerFx != null)
        {
            powerFxAnimator = powerFx.GetComponent<Animator>();
            powerFx.SetActive(false);
        }
    }
    
    public static void ResetRun()
    {
        score = 0;
        gameOver = false;
        slowmo = false;
        zoneTimes.Clear();
    }
    
    // TIEMPO TOTAL
    public static float GetTotalTime()
    {
        float total = 0f;

        foreach (float t in zoneTimes.Values)
        {
            total += t;
        }

        return total;
    }
    
    public static Dictionary<string, float> GetZoneTimes()
    {
        return zoneTimes;
    }

    void Update()
    {
        // Pequeña protección por si estamos en el menú y no hay UI_animator
        if (UI_animator == null || player == null) return;

        bool canRespawn = UI_animator.GetCurrentAnimatorStateInfo(0).IsName("Idle");

        if (Input.GetKeyDown(KeyCode.R) && canRespawn)
        {
            lastRespawnTime = Time.time;
            StartCoroutine(TeleportWithTransition());
        }

        if (Input.GetKeyDown(KeyCode.T) && isSlowMoActivated)
        {
            slowmo = !slowmo;
            Time.timeScale = slowmo ? 0.6f : 1f;
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
        gameOver = false;
        Time.timeScale = 1f;
    }

    IEnumerator TeleportWithTransition()
    {
        player_animator.Play("Player Turn");
        player_script.canMove = false;
        player_rb.simulated = false;
        player_rb.linearVelocity = Vector2.zero;

        UI_animator.SetTrigger("Start");
        yield return new WaitForSeconds(ANIMATION_DURATION);

        RespawnPlayer();
        player_rb.simulated = true;

        UI_animator.SetTrigger("End");
        yield return new WaitForSeconds(ANIMATION_DURATION);
        UI_animator.SetTrigger("BackToIdle");

        player_script.canMove = true;
        player_animator.Play("Player Idle");
    }

    public IEnumerator RespawnPlayerWithTransition()
    {
        player_animator.Play("Player Turn");
        player_sr.enabled = false;
        player_script.canMove = false;
        player_rb.simulated = false;
        player_rb.linearVelocity = Vector2.zero;

        UI_animator.SetTrigger("Start");
        yield return new WaitForSeconds(ANIMATION_DURATION);

        RespawnPlayer();
        player_sr.enabled = true;
        player_rb.simulated = true;

        UI_animator.SetTrigger("End");
        yield return new WaitForSeconds(ANIMATION_DURATION);
        UI_animator.SetTrigger("BackToIdle");

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
        if (Instance == null || chat == null || sprite == null) return;

        Instance.StartCoroutine(Instance.ShowChatRoutine(sprite, fadeIn, hold, fadeOut, scale, offsetX, offsetY));
    }

    private IEnumerator ShowChatRoutine(Sprite sprite, float fadeIn, float hold, float fadeOut,
                                        float scale, float offsetX, float offsetY)
    {
        Transform imgTransform = chat.transform.Find("ImagenMostrar");
        if (imgTransform == null) yield break;

        SpriteRenderer imgRenderer = imgTransform.GetComponent<SpriteRenderer>();
        if (imgRenderer == null) yield break;

        imgRenderer.sprite = sprite;
        imgRenderer.transform.localScale = Vector3.one * scale;
        imgRenderer.transform.localPosition = new Vector3(offsetX, offsetY, imgRenderer.transform.localPosition.z);

        chat.SetActive(true);

        SpriteRenderer[] renderers = chat.GetComponentsInChildren<SpriteRenderer>(true);
        foreach (var r in renderers) { Color c = r.color; c.a = 0f; r.color = c; }

        float t = 0f;
        while (t < fadeIn)
        {
            t += Time.deltaTime;
            float a = t / fadeIn;
            foreach (var r in renderers) { Color c = r.color; c.a = a; r.color = c; }
            yield return null;
        }

        foreach (var r in renderers) { Color c = r.color; c.a = 1f; r.color = c; }
        yield return new WaitForSeconds(hold);

        t = 0f;
        while (t < fadeOut)
        {
            t += Time.deltaTime;
            float a = 1f - t / fadeOut;
            foreach (var r in renderers) { Color c = r.color; c.a = a; r.color = c; }
            yield return null;
        }

        chat.SetActive(false);
    }

   // --------------------------------------------- //

   public static void PowerEffect()
   {
       if(powerFx == null) return;

       powerFx.SetActive(true);
       powerFxAnimator.Play("Power", 0, 0f);       
       Instance.StartCoroutine(DisableFxWhenDone());
   }
   
   private static IEnumerator DisableFxWhenDone()
   {
       if(powerFxAnimator == null) yield break;
       
       yield return new WaitForSeconds(
           powerFxAnimator.GetCurrentAnimatorStateInfo(0).length
       );

       if(powerFx != null) powerFx.SetActive(false);
   }

    void Apply()
    {
        var acc = AccessibilityManager.Instance;
        if (acc == null) return;

        isSlowMoActivated = !isSlowMoActivated;
        if(!isSlowMoActivated)
            Time.timeScale = 1f;
    }
}



