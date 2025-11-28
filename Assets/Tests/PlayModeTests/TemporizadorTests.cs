using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class TemporizadorTests
{
    private GameObject timerObject;
    private Temporizador temporizador;
    private Slider slider;
    private Image relleno;
    private readonly List<Object> toDestroy = new();

    [UnitySetUp]
    public IEnumerator UnitySetUp()
    {
        Time.timeScale = 1f;

        timerObject = new GameObject("Temporizador");
        toDestroy.Add(timerObject);
        temporizador = timerObject.AddComponent<Temporizador>();

        var sliderGO = new GameObject("Slider");
        toDestroy.Add(sliderGO);
        slider = sliderGO.AddComponent<Slider>();

        var imgGO = new GameObject("Relleno");
        toDestroy.Add(imgGO);
        relleno = imgGO.AddComponent<Image>();

        temporizador.sliderTemporizador = slider;
        temporizador.relleno = relleno;
        temporizador.tiempoMaximo = 2f;
        
        yield return null;
    }

    [UnityTearDown]
    public IEnumerator UnityTearDown()
    {
        GameManager.SetGameOver(false);
        Time.timeScale = 1f;

        foreach (var obj in toDestroy)
        {
            if (obj != null) Object.Destroy(obj);
        }
        toDestroy.Clear();
        yield return null;
    }

    [UnityTest]
    public IEnumerator Temporizador_decreases_over_time()
    {
        float inicio = slider.value;
        yield return SimulateRealtime(0.5f);
        Assert.Less(slider.value, inicio, "El slider debe disminuir con el tiempo.");
    }

    [UnityTest]
    public IEnumerator Temporizador_stops_and_gameover_when_time_runs_out()
    {
        temporizador.tiempoMaximo = 0.25f;
        temporizador.ActivarTemporizador();

        yield return SimulateRealtime(0.6f);
        Assert.AreEqual(0f, Time.timeScale, 1e-4f,
            "Cuando el tiempo se agota, debe ponerse el juego en pausa (timeScale=0).");
    }

    [UnityTest]
    public IEnumerator Temporizador_increases_but_caps_at_max()
    {
        temporizador.tiempoMaximo = 3f;
        temporizador.ActivarTemporizador();

        temporizador.AumentarTiempo(5f);

        Assert.That(slider.value,
            Is.EqualTo(temporizador.tiempoMaximo).Within(0.01f),
            "El tiempo no debe superar el máximo configurado.");
        yield break;
    }
    
    private IEnumerator SimulateRealtime(float seconds)
    {
        float end = Time.realtimeSinceStartup + seconds;
        while (Time.realtimeSinceStartup < end)
            yield return null;
    }
}
