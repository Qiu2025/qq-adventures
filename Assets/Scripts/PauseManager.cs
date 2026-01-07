using UnityEngine;
using UnityEngine.SceneManagement; 

// Script del menu de pausa al presionar ESC
public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public string menuSceneName = "Menu"; 
    private bool isPaused = false;

    void Start()
    {
        pauseMenu.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        isPaused = false; 
        SceneManager.LoadScene(menuSceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Saliendo...");
        Application.Quit();
    }
}