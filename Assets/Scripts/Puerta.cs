using UnityEngine;
using UnityEngine.SceneManagement;

// Script de las puertas de paso de nivel
public class Puerta : MonoBehaviour
{
    public string sceneName;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
