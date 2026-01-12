using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class AccessibilityManagerTests
{
    readonly System.Collections.Generic.List<Object> toDestroy = new();

    static FieldInfo InstanceField =>
        typeof(AccessibilityManager).GetField("<Instance>k__BackingField", BindingFlags.Static | BindingFlags.NonPublic);

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        PlayerPrefs.DeleteKey("max_saltos");
        PlayerPrefs.Save();

        ResetSingleton();

        yield return null;
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        PlayerPrefs.DeleteKey("max_saltos");
        PlayerPrefs.Save();

        foreach (var o in toDestroy)
            if (o != null) Object.DestroyImmediate(o);
        toDestroy.Clear();

        ResetSingleton();

        yield return null;
    }

    [UnityTest]
    public IEnumerator Singleton_instance_is_set_and_second_is_destroyed()
    {
        var firstGO = CreateManagerGO("AM_1");
        var first = firstGO.GetComponent<AccessibilityManager>();
        Assert.AreSame(first, AccessibilityManager.Instance);

        var secondGO = CreateManagerGO("AM_2");
        var second = secondGO.GetComponent<AccessibilityManager>();
        yield return null;

        Assert.AreSame(first, AccessibilityManager.Instance);
        Assert.IsTrue(second == null);
    }

    [UnityTest]
    public IEnumerator MasSaltos_loads_from_PlayerPrefs_on_awake()
    {
        PlayerPrefs.SetInt("max_saltos", 0);
        PlayerPrefs.Save();

        var go0 = CreateManagerGO("AM_0");
        var m0 = go0.GetComponent<AccessibilityManager>();
        Assert.IsFalse(m0.MasSaltos);

        Object.DestroyImmediate(go0);
        toDestroy.Remove(go0);
        ResetSingleton();

        PlayerPrefs.SetInt("max_saltos", 1);
        PlayerPrefs.Save();

        var go1 = CreateManagerGO("AM_1");
        var m1 = go1.GetComponent<AccessibilityManager>();
        Assert.IsTrue(m1.MasSaltos);

        yield return null;
    }

    [UnityTest]
    public IEnumerator GetMaxJumps_returns_normal_when_MasSaltos_is_false()
    {
        var go = CreateManagerGO("AM");
        var m = go.GetComponent<AccessibilityManager>();

        m.SetMasSaltos(false);
        Assert.AreEqual(2, m.GetMaxJumps());

        yield return null;
    }

    [UnityTest]
    public IEnumerator GetMaxJumps_returns_accessibility_when_MasSaltos_is_true()
    {
        var go = CreateManagerGO("AM");
        var m = go.GetComponent<AccessibilityManager>();

        m.SetMasSaltos(true);
        Assert.AreEqual(4, m.GetMaxJumps());

        yield return null;
    }

    [UnityTest]
    public IEnumerator SetMasSaltos_saves_to_PlayerPrefs_and_invokes_event()
    {
        var go = CreateManagerGO("AM");
        var m = go.GetComponent<AccessibilityManager>();

        var invoked = false;
        m.OnChanged += () => invoked = true;

        m.SetMasSaltos(true);

        Assert.IsTrue(m.MasSaltos);
        Assert.AreEqual(1, PlayerPrefs.GetInt("max_saltos", 0));
        Assert.IsTrue(invoked);

        yield return null;
    }

    [UnityTest]
    public IEnumerator SetCamaraLenta_invokes_OnChangedSlowMo_event()
    {
        var go = CreateManagerGO("AM");
        var m = go.GetComponent<AccessibilityManager>();

        var invoked = false;
        m.OnChangedSlowMo += () => invoked = true;

        m.SetCamaraLenta(true);

        Assert.IsTrue(invoked);
        yield return null;
    }

    [UnityTest]
    public IEnumerator SetAutopilot_invokes_OnChangedAutopilot_event()
    {
        var go = CreateManagerGO("AM");
        var m = go.GetComponent<AccessibilityManager>();

        var invoked = false;
        m.OnChangedAutopilot += () => invoked = true;

        m.SetAutopilot(true);

        Assert.IsTrue(invoked);
        yield return null;
    }

    GameObject CreateManagerGO(string name)
    {
        var go = new GameObject(name);
        toDestroy.Add(go);
        go.AddComponent<AccessibilityManager>();
        return go;
    }

    static void ResetSingleton()
    {
        var all = Object.FindObjectsByType<AccessibilityManager>(FindObjectsSortMode.None);
        foreach (var a in all)
            if (a != null) Object.DestroyImmediate(a.gameObject);

        InstanceField?.SetValue(null, null);
    }
}
