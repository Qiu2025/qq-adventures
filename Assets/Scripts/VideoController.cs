using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class VideoController : MonoBehaviour
{

    [SerializeField] private GameObject mapPanel;
    [SerializeField] private VideoPlayer vp;
    [SerializeField] private float durationFade;
    
    void Start()
    {
        mapPanel.SetActive(false);
        vp.loopPointReached += OnFinish;
    }

    void OnFinish(VideoPlayer vp)
    {
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        float tiempo = 0;

        while (tiempo < durationFade)
        {
            tiempo += Time.deltaTime;
            vp.targetCameraAlpha = Mathf.Lerp(1f, 0f, tiempo / durationFade);
            yield return null;
        }

        mapPanel.SetActive(true);
        vp.gameObject.SetActive(false);
    }
}
