using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ThemeSwitcherTests
{
    private GameObject switcherGO;
    private ThemeSwitcher switcher;
    private GameObject spring;
    private GameObject bosque;
    private readonly List<Object> toDestroy = new();

    [UnitySetUp]
    public IEnumerator UnitySetUp()
    {
        spring = new GameObject("Spring");
        toDestroy.Add(spring);
        spring.AddComponent<SpriteRenderer>();

        bosque = new GameObject("Bosque");
        toDestroy.Add(bosque);
        bosque.AddComponent<SpriteRenderer>();

        switcherGO = new GameObject("ThemeSwitcher");
        toDestroy.Add(switcherGO);

        switcher = switcherGO.AddComponent<ThemeSwitcher>();
        switcher.enabled = false;

        switcher.Spring = spring;
        switcher.Bosque = bosque;

        // pequeña espera para poder comprobar estado inicial
        switcher.tiempoEspera = 0.1f;
        switcher.duracionTransicion = 0f;

        switcher.enabled = true;
        yield return null;
    }

    [UnityTearDown]
    public IEnumerator UnityTearDown()
    {
        foreach (var o in toDestroy)
            if (o != null) Object.Destroy(o);
        toDestroy.Clear();
        yield return null;
    }

    [UnityTest]
    public IEnumerator Initial_state_is_correct()
    {
        Assert.IsTrue(spring.activeSelf, "Spring debe empezar activo.");
        Assert.AreEqual(1f, spring.GetComponent<SpriteRenderer>().color.a, 0.001f);

        Assert.IsFalse(bosque.activeSelf, "Bosque debe empezar inactivo.");

        yield break;
    }

    [UnityTest]
    public IEnumerator Final_state_after_transition_is_correct()
    {
        // esperar más que tiempoEspera para que la corrutina termine
        yield return new WaitForSeconds(0.2f);

        Assert.IsFalse(spring.activeSelf, "Spring debe desactivarse tras la transición.");
        Assert.IsTrue(bosque.activeSelf, "Bosque debe activarse tras la transición.");

        float a = bosque.GetComponent<SpriteRenderer>().color.a;
        Assert.AreEqual(1f, a, 0.001f, "Bosque debe quedar con alpha = 1.");
    }
}
