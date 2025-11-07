using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static bool gameOver = false;
    public static int score = 0;
    
    /* PARA RESPAWN EN CHECKPOINT */
    private static Vector2 lastCheckpointPos = new Vector2(-11.75f, 6.4f); 
    private static GameObject player;  

    void Awake()
    {
        gameOver = false;
        Time.timeScale = 1f;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (gameOver && Input.GetKeyDown(KeyCode.R))
        {
            ChangeGameOverStatus();
            SceneManager.LoadScene("Desert");
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            RespawnPlayer();
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

    private static void ChangeGameOverStatus()
    {
        gameOver = !gameOver;
        Time.timeScale = gameOver ? 0f : 1f;
    }

    public static void SetGameOver(bool cond) {
        gameOver = cond;
        Time.timeScale = 0f;
    }
}
