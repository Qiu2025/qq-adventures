using UnityEngine;
using TMPro;

public class CronometroUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textoTiempo;
    private float tiempoTranscurrido;

    void Update()
    {
        tiempoTranscurrido += Time.deltaTime;
        if (textoTiempo != null)
            textoTiempo.text = "  " + tiempoTranscurrido.ToString("F2");
    }

    public float GetTiempo() => tiempoTranscurrido;
}