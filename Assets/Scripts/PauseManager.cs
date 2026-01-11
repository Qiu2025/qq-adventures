using UnityEngine;
using UnityEngine.SceneManagement; 

// Script del menu de pausa al presionar ESC
public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject panelMenu;
    public GameObject panelOpciones;
    public GameObject panelSonido;
    public GameObject panelControles;
    public GameObject panelAccesibilidad;

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

    public void Pause()
    {
        pauseMenu.SetActive(true);
        panelMenu.SetActive(true);
        panelOpciones.SetActive(false);
        panelSonido.SetActive(false);
        panelControles.SetActive(false);
        panelAccesibilidad.SetActive(false);

        Time.timeScale = 0f;
        isPaused = true;
    }

    public void OpenOptions()
    {
        panelMenu.SetActive(false);
        panelOpciones.SetActive(true);
    }

    public void OpenSoundOptions()
    {
        panelOpciones.SetActive(false);
        panelSonido.SetActive(true);
    }
    public void OpenControls()
    {
        panelOpciones.SetActive(false);
        panelControles.SetActive(true);
    }
    public void OpenAccesibilityOptions()
    {
        panelOpciones.SetActive(false);
        panelAccesibilidad.SetActive(true);
    }

    public void BackToMenu()
    {
        panelOpciones.SetActive(false);
        panelMenu.SetActive(true);
    }

    public void BackToOptionsMenu()
    {
        panelSonido.SetActive(false);
        panelOpciones.SetActive(true);
    }

    public void BackToOptionsMenuFromControls()
    {
        panelControles.SetActive(false);
        panelOpciones.SetActive(true);
    }

    public void BackToOptionsMenuFromAccesibilityMenu()
    {
        panelAccesibilidad.SetActive(false);
        panelOpciones.SetActive(true);
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