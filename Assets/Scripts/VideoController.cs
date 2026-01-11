using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class IntroVideoInSelector : MonoBehaviour
{
    [Header("Referencias")]
    public VideoPlayer vp;
    public GameObject mapPanel;   // el panel UI del selector (para ocultarlo mientras el vídeo)

    [Header("Escena al terminar")]
    public string firstLevelScene = "1. Salto";

    void Start()
    {
        // 1) Si NO vienes del botón Play -> no hagas nada especial
        if (!SessionFlags.cameFromPlay)
        {
            ShowSelector();
            return;
        }

        // Consumimos el flag para que, aunque vuelvas a esta escena,
        // no se considere "vengo de Play" otra vez.
        SessionFlags.cameFromPlay = false;

        // 2) Si ya se hizo la intro en esta ejecución -> no video, no salto
        if (SessionFlags.introAlreadyDone)
        {
            ShowSelector();
            return;
        }

        // 3) Primera vez REAL de esta ejecución -> reproducir video y luego saltar al nivel 1
        SessionFlags.introAlreadyDone = true;

        if (mapPanel != null) mapPanel.SetActive(false);

        vp.loopPointReached += OnVideoFinished;
        vp.Play();
    }

    private void OnVideoFinished(VideoPlayer v)
    {
        SceneManager.LoadScene(firstLevelScene);
    }

    private void ShowSelector()
    {
        if (vp != null) vp.Stop();
        if (mapPanel != null) mapPanel.SetActive(true);
    }
}
