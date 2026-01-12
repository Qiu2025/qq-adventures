using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CronometroUITests
{
    private GameObject canvasGO;
    private GameObject textoGO;
    private GameObject cronometroGO;
    private GameObject eventSystemGO;

    private CronometroUI cronometro;
    private TextMeshProUGUI textoTiempo;

    private readonly List<Object> toDestroy = new();

    [UnitySetUp]
    public IEnumerator UnitySetUp()
    {
        // EventSystem (evita ciertos problemas en PlayMode con UI)
        eventSystemGO = new GameObject("EventSystem");
        toDestroy.Add(eventSystemGO);
        eventSystemGO.AddComponent<EventSystem>();
        eventSystemGO.AddComponent<StandaloneInputModule>();

        // Canvas
        canvasGO = new GameObject("Canvas");
        toDestroy.Add(canvasGO);
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();

        // Texto TMP
        textoGO = new GameObject("TextoTiempo");
        toDestroy.Add(textoGO);
        textoGO.transform.SetParent(canvasGO.transform, false);
        textoTiempo = textoGO.AddComponent<TextMeshProUGUI>();
        textoTiempo.text = ""; // estado inicial controlado

        // CronometroUI
        cronometroGO = new GameObject("CronometroUI");
        toDestroy.Add(cronometroGO);
        cronometro = cronometroGO.AddComponent<CronometroUI>();

        // Inyectar referencia privada [SerializeField]
        var field = typeof(CronometroUI).GetField("textoTiempo",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field.SetValue(cronometro, textoTiempo);

        // Dejar que Unity ejecute al menos un frame (Awake/Start + primer Update posible)
        yield return null;
    }

    [UnityTearDown]
    public IEnumerator UnityTearDown()
    {
        foreach (var obj in toDestroy)
        {
            if (obj != null) Object.Destroy(obj);
        }
        toDestroy.Clear();
        yield return null;
    }

    [UnityTest]
    public IEnumerator GetTiempo_increases_over_time()
    {
        // Espera un frame para tener un baseline estable
        yield return null;

        float tiempoInicial = cronometro.GetTiempo();

        yield return new WaitForSeconds(0.5f);

        float tiempoFinal = cronometro.GetTiempo();

        Assert.Greater(tiempoFinal, tiempoInicial,
            "GetTiempo debe aumentar con el tiempo transcurrido.");

        // Margen más realista en PlayMode
        Assert.That(tiempoFinal, Is.GreaterThanOrEqualTo(0.35f),
            "Tras ~0.5s, el tiempo debería ser al menos ~0.35s (depende del framerate).");
    }

    [UnityTest]
    public IEnumerator Texto_tiempo_is_updated_in_update()
    {
        // Espera un poco para que Update escriba el texto varias veces
        yield return new WaitForSeconds(0.2f);

        string txt = textoTiempo.text;
        Assert.IsNotNull(txt, "El texto no debe ser null.");
        Assert.IsNotEmpty(txt.Trim(), "El texto del tiempo no debe estar vacío.");

        // El formato es "  " + tiempo en F2. Extraemos el número robustamente.
        string numberPart = txt.Trim();

        // Algunas culturas usan coma: aquí el script usa ToString("F2") con cultura actual.
        // Para hacerlo robusto, aceptamos tanto coma como punto convirtiendo coma a punto.
        numberPart = numberPart.Replace(',', '.');

        Assert.IsTrue(float.TryParse(numberPart, System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out float value),
            $"El texto debería ser parseable como float. Texto actual: '{txt}'");

        Assert.Greater(value, 0f, "El valor mostrado debería ser > 0 tras esperar.");
    }

    [UnityTest]
    public IEnumerator GetTiempo_is_roughly_accurate()
    {
        yield return null; // baseline
        yield return new WaitForSeconds(1f);

        float tiempo = cronometro.GetTiempo();

        // En PlayMode puede variar (carga editor/CI), así que un rango razonable:
        Assert.That(tiempo, Is.GreaterThan(0.7f).And.LessThan(1.6f),
            $"Después de ~1s, GetTiempo debería estar cerca de 1. Tiempo real: {tiempo:F2}");
    }
}
