using UnityEngine;
using UnityEngine.UI;

// DEPRECATED
// Script para controlar el temporizador (relacionado con el helado)
public class Temporizador : MonoBehaviour
{
    public float tiempoMaximo;
    public Slider sliderTemporizador;
    public Image relleno;
    private float tiempoActual;
    private bool temporizadorActivo = false;

    private void Start()
    {
        ActivarTemporizador();
    }

    private void Update()
    {
        if (temporizadorActivo)
        {
            Time.timeScale = 1f;
            CambiarContador();
        }
        else
        {
            GameManager.SetGameOver(true);
            Debug.Log("You died!");
        }
    }

    private void CambiarContador()
    {
        tiempoActual -= Time.deltaTime;
        if (tiempoActual >= 0)
        {
            sliderTemporizador.value = tiempoActual;
            float porcentaje = tiempoActual / tiempoMaximo;
            relleno.color = Color.Lerp(Color.red, Color.green, porcentaje);
        }

        if (tiempoActual <= 0)
        {
            temporizadorActivo = false;
        }
    }

    private void CambiarTemporizador(bool estado)
    {
        temporizadorActivo = estado;
    }

    public void ActivarTemporizador()
    {
        tiempoActual = tiempoMaximo;
        sliderTemporizador.maxValue = tiempoMaximo;
        CambiarTemporizador(true);
    }

    public void DesactivarTemporizador()
    {
        CambiarTemporizador(false);
    }

    public void AumentarTiempo(float segundos)
    {
        // Suma tiempo y limita al máximo (si no quieres límite, comenta la siguiente línea)
        tiempoActual += segundos;
        if (tiempoActual > tiempoMaximo) tiempoActual = tiempoMaximo;

        // Si estaba parado y ahora tiene tiempo > 0, reanuda
        if (!temporizadorActivo && tiempoActual > 0f)
            temporizadorActivo = true;

        // Actualiza el slider inmediatamente
        sliderTemporizador.value = tiempoActual;
    }

}
