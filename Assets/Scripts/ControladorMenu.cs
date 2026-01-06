using UnityEngine;
using UnityEngine.SceneManagement; 

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