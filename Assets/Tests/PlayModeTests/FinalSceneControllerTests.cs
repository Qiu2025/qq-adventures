using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class FinalSceneControllerTests
{
    GameObject controllerGO;
    FinalSceneController controller;
    Dictionary<string, float> zoneTimesFieldRef;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        controllerGO = new GameObject("FinalSceneController");
        controller = controllerGO.AddComponent<FinalSceneController>();
        controller.enabled = false;

        ResetGameManagerStatics();

        yield return null;
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        ResetGameManagerStatics();
        Object.Destroy(controllerGO);
        yield return null;
    }

    [UnityTest]
    public IEnumerator BuildStatsText_incluye_tiempos_por_zona()
    {
        SetZoneTimes(new Dictionary<string, float> { { "1. Salto", 10.0f }, { "DobleSalto", 20.0f } });
        SetScore(0);

        var text = InvokePrivate<string>("BuildStatsText");

        Assert.IsTrue(text.Contains("Zona Salto:"), "Debe incluir la zona Salto.");
        Assert.IsTrue(text.Contains("Zona Doble Salto:"), "Debe incluir la zona Doble Salto.");

        yield return null;
    }

    [UnityTest]
    public IEnumerator FormatTime_formatea_correctamente()
    {
        Assert.AreEqual("00:00.00", InvokePrivate<string>("FormatTime", 0f));
        Assert.AreEqual("01:05.75", InvokePrivate<string>("FormatTime", 65.75f));
        Assert.AreEqual("02:05.50", InvokePrivate<string>("FormatTime", 125.50f));
        yield return null;
    }

    [UnityTest]
    public IEnumerator CleanZoneKey_limpia_prefijo_y_espacios()
    {
        Assert.AreEqual("Salto", InvokePrivate<string>("CleanZoneKey", "1. Salto"));
        Assert.AreEqual("Salto", InvokePrivate<string>("CleanZoneKey", "Salto"));
        Assert.AreEqual("DobleSalto", InvokePrivate<string>("CleanZoneKey", "   DobleSalto   "));
        yield return null;
    }

    [UnityTest]
    public IEnumerator GetZoneTitle_devuelve_titulos_esperados()
    {
        Assert.AreEqual("Zona Salto", InvokePrivate<string>("GetZoneTitle", "Salto"));
        Assert.AreEqual("Zona Doble Salto", InvokePrivate<string>("GetZoneTitle", "DobleSalto"));
        Assert.AreEqual("Zona Dash", InvokePrivate<string>("GetZoneTitle", "Dash"));
        Assert.AreEqual("Zona Power Ups", InvokePrivate<string>("GetZoneTitle", "PowerUps"));
        Assert.AreEqual("Zona CustomZone", InvokePrivate<string>("GetZoneTitle", "CustomZone"));
        yield return null;
    }

    T InvokePrivate<T>(string methodName, params object[] args)
    {
        var m = typeof(FinalSceneController).GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(m, $"Método '{methodName}' no encontrado.");
        return (T)m.Invoke(controller, args);
    }

    static void ResetGameManagerStatics()
    {
        var t = typeof(GameManager);
        var flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        var scoreF = t.GetField("score", flags);
        if (scoreF != null && scoreF.FieldType == typeof(int)) scoreF.SetValue(null, 0);

        var zoneTimesF = t.GetField("zoneTimes", flags);
        if (zoneTimesF != null)
        {
            if (zoneTimesF.GetValue(null) is IDictionary dict) dict.Clear();
            else if (zoneTimesF.FieldType.IsGenericType &&
                     zoneTimesF.FieldType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                var inst = System.Activator.CreateInstance(zoneTimesF.FieldType);
                zoneTimesF.SetValue(null, inst);
            }
        }
    }

    static void SetScore(int v)
    {
        var t = typeof(GameManager);
        var flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        var f = t.GetField("score", flags);
        if (f != null && f.FieldType == typeof(int)) f.SetValue(null, v);
    }

    static void SetZoneTimes(Dictionary<string, float> values)
    {
        var t = typeof(GameManager);
        var flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        var f = t.GetField("zoneTimes", flags);

        if (f == null) return;

        var current = f.GetValue(null);
        if (current is IDictionary dict)
        {
            dict.Clear();
            foreach (var kv in values) dict[kv.Key] = kv.Value;
            return;
        }

        if (f.FieldType == typeof(Dictionary<string, float>))
        {
            f.SetValue(null, values);
            return;
        }

        var inst = System.Activator.CreateInstance(f.FieldType);
        var add = f.FieldType.GetMethod("Add", new[] { typeof(string), typeof(float) });
        if (add != null)
        {
            foreach (var kv in values) add.Invoke(inst, new object[] { kv.Key, kv.Value });
            f.SetValue(null, inst);
        }
    }
}
