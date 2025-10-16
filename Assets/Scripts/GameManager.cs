using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static bool gameOver = false;

    void Update()
    {
        if (gameOver && Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("DesertSampleScene");
            ChangeGameOverStatus();
        }
    }

    public static void ChangeGameOverStatus()
    {
        gameOver = !gameOver;
        Time.timeScale = gameOver ? 0f : 1f;
    }

    public static void SetGameOver(bool cond) {
        Time.timeScale = cond ? 0f : 1f;
        gameOver = cond;
    }
}
