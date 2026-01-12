using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class DashCooldownBarTests
{
    GameObject canvasGO;
    GameObject bgGO;
    GameObject fillGO;
    GameObject rootToHide;
    GameObject barGO;

    DashCooldownBar dashCooldownBar;
    RectTransform bgRect;
    RectTransform fillRect;

    readonly List<Object> toDestroy = new();

    [UnitySetUp]
    public IEnumerator UnitySetUp()
    {
        canvasGO = new GameObject("Canvas");
        toDestroy.Add(canvasGO);
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();

        bgGO = new GameObject("BarBG");
        toDestroy.Add(bgGO);
        bgGO.transform.SetParent(canvasGO.transform, false);
        bgRect = bgGO.AddComponent<RectTransform>();
        bgRect.anchorMin = bgRect.anchorMax = new Vector2(0.5f, 0.5f);
        bgRect.pivot = new Vector2(0.5f, 0.5f);
        bgRect.sizeDelta = new Vector2(200f, 20f);
        bgRect.anchoredPosition = Vector2.zero;

        fillGO = new GameObject("BarFill");
        toDestroy.Add(fillGO);
        fillGO.transform.SetParent(canvasGO.transform, false);
        fillRect = fillGO.AddComponent<RectTransform>();
        fillRect.anchorMin = fillRect.anchorMax = new Vector2(0.5f, 0.5f);
        fillRect.pivot = new Vector2(0.5f, 0.5f);
        fillRect.sizeDelta = new Vector2(200f, 20f);
        fillRect.anchoredPosition = Vector2.zero;

        rootToHide = new GameObject("RootToHide");
        toDestroy.Add(rootToHide);
        rootToHide.transform.SetParent(canvasGO.transform, false);
        rootToHide.SetActive(true);

        barGO = new GameObject("DashCooldownBar");
        toDestroy.Add(barGO);

        dashCooldownBar = barGO.AddComponent<DashCooldownBar>();
        dashCooldownBar.enabled = false;

        var flags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
        typeof(DashCooldownBar).GetField("fillRect", flags).SetValue(dashCooldownBar, fillRect);
        typeof(DashCooldownBar).GetField("bgRect", flags).SetValue(dashCooldownBar, bgRect);
        typeof(DashCooldownBar).GetField("rootToHide", flags).SetValue(dashCooldownBar, rootToHide);

        dashCooldownBar.enabled = true;

        yield return null;
        Canvas.ForceUpdateCanvases();
        yield return null;
    }

    [UnityTearDown]
    public IEnumerator UnityTearDown()
    {
        foreach (var obj in toDestroy)
            if (obj != null) Object.Destroy(obj);
        toDestroy.Clear();
        yield return null;
    }

    [UnityTest]
    public IEnumerator SetReady01_clamps_values_outside_01_range()
    {
        dashCooldownBar.SetReady01(-0.5f);
        yield return null;
        Assert.AreEqual(200f, fillRect.sizeDelta.x, 0.1f);

        dashCooldownBar.SetReady01(1.5f);
        yield return null;
        Assert.AreEqual(0f, fillRect.sizeDelta.x, 0.1f);
    }

    [UnityTest]
    public IEnumerator SetReady01_at_zero_shows_full_bar()
    {
        dashCooldownBar.SetReady01(0f);
        yield return null;
        Assert.AreEqual(200f, fillRect.sizeDelta.x, 0.1f);
    }

    [UnityTest]
    public IEnumerator SetReady01_at_one_hides_bar()
    {
        dashCooldownBar.SetReady01(1f);
        yield return null;

        Assert.AreEqual(0f, fillRect.sizeDelta.x, 0.1f);
        Assert.IsFalse(rootToHide.activeSelf);
    }

    [UnityTest]
    public IEnumerator SetReady01_at_half_shows_half_bar()
    {
        dashCooldownBar.SetReady01(0.5f);
        yield return null;

        Assert.AreEqual(100f, fillRect.sizeDelta.x, 0.1f);
        Assert.IsTrue(rootToHide.activeSelf);
    }

    [UnityTest]
    public IEnumerator OnDashUsed_sets_bar_to_zero()
    {
        dashCooldownBar.SetReady01(1f);
        yield return null;

        dashCooldownBar.OnDashUsed();
        yield return null;

        Assert.AreEqual(200f, fillRect.sizeDelta.x, 0.1f);
        Assert.IsTrue(rootToHide.activeSelf);
    }

    [UnityTest]
    public IEnumerator OnReady_sets_bar_to_one()
    {
        dashCooldownBar.SetReady01(0f);
        yield return null;

        dashCooldownBar.OnReady();
        yield return null;

        Assert.AreEqual(0f, fillRect.sizeDelta.x, 0.1f);
        Assert.IsFalse(rootToHide.activeSelf);
    }
}
