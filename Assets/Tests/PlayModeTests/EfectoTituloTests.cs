using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class EfectoTituloTests
{
    private readonly List<Object> toDestroy = new();

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
    public IEnumerator SecuenciaTitulo_fadea_y_desactiva_el_objeto()
    {
        var go = new GameObject("Titulo");
        toDestroy.Add(go);

        var canvasGroup = go.AddComponent<CanvasGroup>();
        var effect = go.AddComponent<EfectoTitulo>();
        effect.canvasGroup = canvasGroup;

        effect.espera = 0f;                 // 👈 CLAVE
        effect.tiempoAparicion = 0.05f;
        effect.tiempoEspera = 0.05f;
        effect.tiempoDesaparicion = 0.05f;

        yield return null;                  // deja que corra Start()

        yield return new WaitForSeconds(0.03f); // a mitad del fade-in aprox
        Assert.Greater(canvasGroup.alpha, 0f, "Durante el fade in el alpha debe incrementarse.");

        yield return new WaitForSeconds(0.2f);
        Assert.AreEqual(0f, canvasGroup.alpha, 0.001f, "Tras la secuencia el alpha debe volver a 0.");
        Assert.IsFalse(go.activeSelf, "El objeto debe desactivarse al finalizar la secuencia.");
    }


    [UnityTest]
    public IEnumerator Start_sin_canvasGroup_registra_error()
    {
        var go = new GameObject("Titulo");
        toDestroy.Add(go);

        LogAssert.Expect(LogType.Error, "Falta asignar el CanvasGroup en el inspector");
        go.AddComponent<EfectoTitulo>();
        yield return null;
    }
}

