using UnityEngine;
using UnityEngine.SceneManagement; 

// Script del menu de pausa al presionar ESC
public class PauseManager1 : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject panelMenu;
    public GameObject panelOpciones;
    public GameObject panelSonido;
    public GameObject panelControles;

    void Start()
    {
        pauseMenu.SetActive(false);
        Pause();
    }
    public void Play()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MapSelection");
    }

    public void Pause()
    {
        pauseMenu.SetActive(true);
        panelMenu.SetActive(true);
        panelOpciones.SetActive(false);
        panelSonido.SetActive(false);
        panelControles.SetActive(false);

        Time.timeScale = 0f;
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


    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    public void QuitGame()
    {
        Debug.Log("Saliendo...");
        Application.Quit();
    }
}