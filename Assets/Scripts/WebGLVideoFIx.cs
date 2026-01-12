using UnityEngine;
using UnityEngine.Video;

public class WebGLVideoFix : MonoBehaviour
{
    [SerializeField] private string nombreDelVideo = "cinematica_inicial.mp4";
    private VideoPlayer videoPlayer;

    void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();

        // Construimos la ruta correcta hacia la carpeta StreamingAssets
        string videoUrl = System.IO.Path.Combine(Application.streamingAssetsPath, nombreDelVideo);
        
        // Asignamos la URL al reproductor
        videoPlayer.url = videoUrl;
        
        videoPlayer.Prepare();
    }
}