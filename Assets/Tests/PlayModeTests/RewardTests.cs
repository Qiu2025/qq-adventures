using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class RewardTests
{
    private readonly List<Object> toDestroy = new();
    private GameObject rewardObject;
    private GameObject player;
    private TextMeshProUGUI scoreText;

    [UnitySetUp]
    public IEnumerator UnitySetUp()
    {
        GameManager.score = 0;

        EnsureMainCameraWithAudioListener();
        EnsureAudioManagerReady();

        var canvasGO = new GameObject("ScoreCanvas");
        toDestroy.Add(canvasGO);
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();

        var scoreGO = new GameObject("Score");
        toDestroy.Add(scoreGO);
        scoreGO.tag = "Score";
        scoreGO.transform.SetParent(canvasGO.transform, false);
        scoreText = scoreGO.AddComponent<TextMeshProUGUI>();
        scoreText.text = "0";

        player = new GameObject("Player");
        toDestroy.Add(player);
        player.tag = "Player";
        player.AddComponent<BoxCollider2D>();

        rewardObject = new GameObject("Reward");
        toDestroy.Add(rewardObject);
        var rewardCol = rewardObject.AddComponent<BoxCollider2D>();
        rewardCol.isTrigger = true;
        rewardObject.AddComponent<SpriteRenderer>();
        rewardObject.AddComponent<Reward>();

        yield return null; // Start() de Reward
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
    public IEnumerator Reward_incrementa_score_y_se_destruye_al_recogerlo()
    {
        var reward = rewardObject.GetComponent<Reward>();

        var onTrigger = typeof(Reward).GetMethod("OnTriggerEnter2D", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(onTrigger);

        onTrigger.Invoke(reward, new object[] { player.GetComponent<Collider2D>() });

        Assert.AreEqual(1, GameManager.score);
        Assert.AreEqual("  1 / ?", scoreText.text);

        float timeout = Time.time + 1.25f;
        while (rewardObject != null && Time.time < timeout)
            yield return null;

        Assert.IsTrue(rewardObject == null);
    }

    private void EnsureMainCameraWithAudioListener()
    {
        var cam = Camera.main;
        if (cam == null)
        {
            var camGO = new GameObject("Main Camera");
            toDestroy.Add(camGO);
            camGO.tag = "MainCamera";
            cam = camGO.AddComponent<Camera>();
        }

        if (cam.GetComponent<AudioListener>() == null)
            cam.gameObject.AddComponent<AudioListener>();
    }

    private void EnsureAudioManagerReady()
    {
        var existing = Object.FindObjectOfType<AudioManager>();
        AudioManager am;

        if (existing != null) am = existing;
        else
        {
            var amGO = new GameObject("AudioManager_Test");
            toDestroy.Add(amGO);
            am = amGO.AddComponent<AudioManager>();
        }

        // Asegura AudioSource
        var src = am.GetComponent<AudioSource>();
        if (src == null) src = am.gameObject.AddComponent<AudioSource>();

        // Intenta setear Instance si existe
        ForceSetSingletonInstance(am);

        // Autorellena campos típicos para evitar nulls dentro de PlayCoinSound()
        AutoWireAudioFields(am, src);
    }

    private static void ForceSetSingletonInstance(AudioManager am)
    {
        var t = typeof(AudioManager);
        const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

        var f = t.GetField("Instance", flags);
        if (f != null && typeof(AudioManager).IsAssignableFrom(f.FieldType))
        {
            f.SetValue(null, am);
            return;
        }

        var p = t.GetProperty("Instance", flags);
        if (p != null && p.CanWrite && typeof(AudioManager).IsAssignableFrom(p.PropertyType))
        {
            p.SetValue(null, am);
        }
    }

    private static void AutoWireAudioFields(AudioManager am, AudioSource src)
    {
        var t = am.GetType();
        const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        var fields = t.GetFields(flags);

        foreach (var f in fields)
        {
            if (f.FieldType == typeof(AudioSource))
            {
                if (f.GetValue(am) == null) f.SetValue(am, src);
                continue;
            }

            if (f.FieldType == typeof(AudioClip))
            {
                if (f.GetValue(am) == null)
                    f.SetValue(am, AudioClip.Create("DummyClip", 4410, 1, 44100, false));
                continue;
            }

            if (f.FieldType == typeof(AudioClip[]))
            {
                if (f.GetValue(am) == null)
                    f.SetValue(am, new[] { AudioClip.Create("DummyClip", 4410, 1, 44100, false) });
                continue;
            }
        }

        // Si AudioManager usa propiedades en vez de fields, intenta lo mismo con properties
        var props = t.GetProperties(flags).Where(p => p.CanWrite);
        foreach (var p in props)
        {
            if (p.PropertyType == typeof(AudioSource))
            {
                if (p.GetValue(am) == null) p.SetValue(am, src);
            }
            else if (p.PropertyType == typeof(AudioClip))
            {
                if (p.GetValue(am) == null)
                    p.SetValue(am, AudioClip.Create("DummyClip", 4410, 1, 44100, false));
            }
        }
    }
}
