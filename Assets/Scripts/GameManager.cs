using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static bool gameOver = false;
    public static int score = 0;

    void Awake()
    {
        gameOver = false;
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (gameOver && Input.GetKeyDown(KeyCode.R))
        {
            ChangeGameOverStatus();
            SceneManager.LoadScene("Desert");
        }
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
