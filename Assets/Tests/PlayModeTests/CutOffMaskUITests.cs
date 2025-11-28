using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class CutOffMaskUITests
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
    public IEnumerator MaterialForRendering_usa_comparacion_NotEqual_para_el_stencil()
    {
        var go = new GameObject("MaskUI");
        toDestroy.Add(go);

        go.AddComponent<RectTransform>();
        go.AddComponent<CanvasRenderer>();

        var image = go.AddComponent<CutOffMaskUI>();
        image.material = new Material(Shader.Find("UI/Default"));

        yield return null;

        Material mat = image.materialForRendering;
        toDestroy.Add(mat);
        Assert.AreEqual((int)CompareFunction.NotEqual, mat.GetInt("_StencilComp"),
            "El material generado debe usar CompareFunction.NotEqual para el stencil.");
    }
}

