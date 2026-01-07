using UnityEngine;
using UnityEngine.SceneManagement; 

// Script que detecta las acciones de jugador en el menu
// Metido en un empty de la escena Menu
public class ControladorMenu : MonoBehaviour
{
    void Update()
    {
        if (Input.anyKeyDown && !Input.GetMouseButton(0) && !Input.GetMouseButton(1) 
            && !Input.GetMouseButton(2) && !Input.GetMouseButton(3) && !Input.GetMouseButton(4) 
            && !Input.GetMouseButton(5) && !Input.GetMouseButton(6))
        {
            CambiarDeEscena();
        }
    }

    void CambiarDeEscena()
    {
        SceneManager.LoadScene("MapSelection");
    }
}