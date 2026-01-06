using UnityEngine;
using TMPro;

public class CronometroUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textoTiempo;
    private float tiempoTranscurrido;

    void Update()
    {
        tiempoTranscurrido += Time.deltaTime;
        textoTiempo.text = "  " + tiempoTranscurrido.ToString("F2");
    }
}