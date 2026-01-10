using UnityEngine;
using UnityEngine.SceneManagement;

public class FirstTimeMapRedirect : MonoBehaviour
{
    [Header("Escenas")]
    [Tooltip("Escena a la que irá la PRIMERA vez de esta ejecución (ej: Tutorial o Level1)")]
    public string firstTimeScene;

    [Tooltip("Escena a la que irá las siguientes veces (ej: InMapLevelSelector)")]
    public string returnScene = "InMapLevelSelector";

    // Se reinicia al cerrar el juego o parar Play
    private static bool hasEnteredThisSession = false;

    public void Go()
    {
        if (!hasEnteredThisSession)
        {
            hasEnteredThisSession = true;

            if (!string.IsNullOrEmpty(firstTimeScene))
                SceneManager.LoadScene(firstTimeScene);
            else
                Debug.LogError("FirstTimeMapRedirect: firstTimeScene está vacío.");
        }
        else
        {
            if (!string.IsNullOrEmpty(returnScene))
                SceneManager.LoadScene(returnScene);
            else
                Debug.LogError("FirstTimeMapRedirect: returnScene está vacío.");
        }
    }

    // Opcional: reset manual mientras estás jugando
    [ContextMenu("Reset Session Flag")]
    public void ResetSessionFlag()
    {
        hasEnteredThisSession = false;
        Debug.Log("FirstTimeMapRedirect: session flag reseteado.");
    }
}
