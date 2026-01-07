using UnityEngine;
using TMPro;

// Script que actualiza el tiempo, mostrado en la HUD de cada nivel
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