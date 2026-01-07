using UnityEngine;
using UnityEngine.SceneManagement; 

// Script del menu de pausa al presionar ESC
public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject panelMenu;
    public GameObject panelOpciones;

    public string menuSceneName = "Menu";

    float originalMusicVolume;

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

        AudioManager.Instance.bgm.volume = originalMusicVolume;
        isPaused = false;
    }

    void Pause()
    {
        pauseMenu.SetActive(true);
        panelMenu.SetActive(true);
        panelOpciones.SetActive(false);

        Time.timeScale = 0f;

        originalMusicVolume = AudioManager.Instance.bgm.volume;
        AudioManager.Instance.bgm.volume = originalMusicVolume * 0.5f;

        isPaused = true;
    }

    public void OpenOptions()
    {
        panelMenu.SetActive(false);
        panelOpciones.SetActive(true);
    }

    public void BackToMenu()
    {
        panelOpciones.SetActive(false);
        panelMenu.SetActive(true);
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