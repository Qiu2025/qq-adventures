using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Script de las puertas, para reproducir una animacion y cargar nueva escena
public class Puerta : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float transitionTime = 1f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;

            if (nextIndex < SceneManager.sceneCountInBuildSettings)
            {
                StartCoroutine(LoadNextLevel(nextIndex));   // Siguiente nivel
            } else
            {
                StartCoroutine(LoadNextLevel(0));   // Si es el ultimo, al menu
            }
        }
    }

    IEnumerator LoadNextLevel(int levelIndex)
    {
        animator.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(levelIndex);
    }
}
