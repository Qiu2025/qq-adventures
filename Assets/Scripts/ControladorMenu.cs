using UnityEngine;
using UnityEngine.SceneManagement; 

public class ControladorMenu : MonoBehaviour
{

    void Update()
    {
        if (Input.anyKeyDown)
        {
            CambiarDeEscena();
        }
    }

    void CambiarDeEscena()
    {
        SceneManager.LoadScene("MapSelection");
    }
}