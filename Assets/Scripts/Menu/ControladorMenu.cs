using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

// Script que detecta las acciones de jugador en el menu
// Metido en un empty de la escena Menu
public class ControladorMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject panelMenu;
    public GameObject panelOpciones;
    public GameObject panelSonido;
    public GameObject panelControles;
    public GameObject panelCreditos;
    public GameObject texto;

    void Start()
    {
        pauseMenu.SetActive(false);
    }
    void Update()
    {
        if (Input.anyKeyDown && !Input.GetMouseButton(0) && !Input.GetMouseButton(1) 
            && !Input.GetMouseButton(2) && !Input.GetMouseButton(3) && !Input.GetMouseButton(4) 
            && !Input.GetMouseButton(5) && !Input.GetMouseButton(6))
        {
            StartCoroutine(OpenMenu());
        }
    }
    
    public void Play()
    {
        SceneManager.LoadScene("InMapLevellSelector");
    }

    public IEnumerator OpenMenu()
    {
        texto.SetActive(false);
        yield return new WaitForSeconds(0.2f);
        pauseMenu.SetActive(true);
        panelMenu.SetActive(true);
        panelOpciones.SetActive(false);
        panelSonido.SetActive(false);
        panelControles.SetActive(false);
        panelCreditos.SetActive(false);
        
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

    public void OpenCredits()
    {
        panelMenu.SetActive(false);
        panelCreditos.SetActive(true);
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

    public void BackToMenuFromCredits()
    {
        panelCreditos.SetActive(false);
        panelMenu.SetActive(true);
    }


    public void BackToOptionsMenuFromControls()
    {
        panelControles.SetActive(false);
        panelOpciones.SetActive(true);
    }



    public void GoToMainMenu()
    {
        pauseMenu.SetActive(false);
        panelMenu.SetActive(false);
        panelOpciones.SetActive(false);
        panelSonido.SetActive(false);
        panelControles.SetActive(false);
        panelCreditos.SetActive(false);
    }

    public void QuitGame()
    {
        Debug.Log("Saliendo...");
        Application.Quit();
    }
}