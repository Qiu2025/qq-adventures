using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // --------------------------------------------- //
    private static bool gameOver = false;
    private static bool slowmo = false;
    
    // --- PARA PUNTUACION POR LOS COLLECTIBLES --- //
    [HideInInspector] public static int score = 0;
    
    // -------- PARA RESPAWN EN CHECKPOINT -------- //
    private static Vector2 lastCheckpointPos = new Vector2(-11.75f, 6.4f); 
    private static GameObject player;

    // --------------------------------------------- //

    [Header("Selección de mecánicas")]
    public bool allowDoubleJump = false;
    public bool allowDash = false;
    public bool allowGlide = false;

    // --------------------------------------------- //

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        Application.targetFrameRate = 144;
        Instance = this;
    }

    void Update()
    {
        // Para volver al ultimo checkpoint
        if (Input.GetKeyDown(KeyCode.R))
        {
            RespawnPlayer();
        }

        // Slow motion, para debug
        if (Input.GetKeyDown(KeyCode.T))
        {
            slowmo = !slowmo;
            Time.timeScale = slowmo ? 0.2f : 1f;
        }
    }
    
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

    public static void SetGameOver(bool cond) {
        gameOver = cond;
        Time.timeScale = gameOver ? 0f: 1f;
    }
}
