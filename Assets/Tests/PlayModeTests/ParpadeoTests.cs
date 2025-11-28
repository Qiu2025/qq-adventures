using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class ParpadeoTests
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
    public IEnumerator Update_modifica_alpha_en_el_rango_indicado()
    {
        var go = CreateImageObject("Blink");
        var effect = go.AddComponent<EfectoParpadeo>();
        effect.velocidad = 15f;
        effect.alphaMinimo = 0.2f;
        effect.alphaMaximo = 0.8f;

        yield return null;

        var image = go.GetComponent<Image>();
        float firstAlpha = image.color.a;
        yield return new WaitForSeconds(0.1f);
        float secondAlpha = image.color.a;

        Assert.AreNotEqual(firstAlpha, secondAlpha, "El alpha debe variar a lo largo del tiempo.");
        Assert.GreaterOrEqual(secondAlpha, effect.alphaMinimo - 0.01f);
        Assert.LessOrEqual(secondAlpha, effect.alphaMaximo + 0.01f);
    }

    [UnityTest]
    public IEnumerator Update_no_falla_si_no_hay_Image()
    {
        var go = new GameObject("BlinkNoImage");
        toDestroy.Add(go);

        var effect = go.AddComponent<EfectoParpadeo>();
        effect.velocidad = 10f;

        // No debe lanzar excepciones aunque falte la Image
        yield return null;
        go.SendMessage("Update");
    }

    private GameObject CreateImageObject(string name)
    {
        var go = new GameObject(name);
        toDestroy.Add(go);
        go.AddComponent<RectTransform>();
        go.AddComponent<CanvasRenderer>();
        go.AddComponent<Image>();
        return go;
    }
}

