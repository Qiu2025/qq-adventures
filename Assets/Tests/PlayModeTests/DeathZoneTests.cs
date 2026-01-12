using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class DeathZoneUltraRobustTests
{
    private readonly List<UnityEngine.Object> toDestroy = new();

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        Physics2D.simulationMode = SimulationMode2D.FixedUpdate;
        Physics2D.gravity = Vector2.zero;
        Time.timeScale = 1f;

        // Limpia singleton previo
        if (GameManager.Instance != null)
        {
            UnityEngine.Object.Destroy(GameManager.Instance.gameObject);
            typeof(GameManager)
                .GetField("<Instance>k__BackingField", BindingFlags.Static | BindingFlags.NonPublic)
                ?.SetValue(null, null);
            yield return null;
        }

        // Player mínimo
        var player = new GameObject("Player");
        toDestroy.Add(player);
        player.tag = "Player";
        player.transform.position = new Vector3(-3f, 0f, 0f);

        player.AddComponent<BoxCollider2D>();
        var rb = player.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;

        player.AddComponent<Animator>();
        player.AddComponent<SpriteRenderer>();

        // UICanvas
        var uiCanvas = new GameObject("UICanvas");
        toDestroy.Add(uiCanvas);
        uiCanvas.tag = "UICanvas";
        uiCanvas.AddComponent<Animator>();

        // Chat
        var chat = new GameObject("Chat");
        toDestroy.Add(chat);
        chat.tag = "Chat";
        var img = new GameObject("ImagenMostrar");
        img.transform.SetParent(chat.transform);
        img.AddComponent<SpriteRenderer>();
        chat.SetActive(false);

        // PowerEffect
        var power = new GameObject("PowerEffect");
        toDestroy.Add(power);
        power.tag = "PowerEffect";
        power.AddComponent<Animator>();
        power.AddComponent<SpriteRenderer>();
        power.SetActive(true);

        // GameManager
        var gmGO = new GameObject("GameManager");
        toDestroy.Add(gmGO);
        gmGO.AddComponent<GameManager>();

        // Deja que Awake/OnEnable corran y hagan RefreshReferences
        yield return null;

        // IMPORTANTE: Si existe PlayerMovement en el proyecto, lo añadimos pero DESHABILITADO
        // para que GameManager tenga referencia y NO explote, y para que Start() NO se ejecute.
        var pmType = FindTypeByName("PlayerMovement");
        if (pmType != null && player.GetComponent(pmType) == null)
        {
            var pm = player.AddComponent(pmType) as Behaviour;
            if (pm != null) pm.enabled = false; // evita Start() que te estaba petando
        }

        GameManager.SetCheckpoint(Vector2.zero);
        yield return null;
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        Time.timeScale = 1f;

        foreach (var o in toDestroy)
            if (o) UnityEngine.Object.Destroy(o);

        toDestroy.Clear();
        yield return null;
    }

    [UnityTest]
    public IEnumerator DeathZone_respawnea_al_player_al_entrar()
    {
        // DeathZone
        var dz = new GameObject("DeathZone");
        toDestroy.Add(dz);
        var dzCol = dz.AddComponent<BoxCollider2D>();
        dzCol.isTrigger = true;

        var dzComp = dz.AddComponent<DeathZone>();

        // Prefab privado para no instanciar null
        var fakeExplosionPrefab = new GameObject("FakeExplosionPrefab");
        toDestroy.Add(fakeExplosionPrefab);
        typeof(DeathZone)
            .GetField("prefabExplosionPlumas", BindingFlags.Instance | BindingFlags.NonPublic)
            ?.SetValue(dzComp, fakeExplosionPrefab);

        // Disparo manual del trigger (sin física)
        var player = GameObject.FindGameObjectWithTag("Player");
        Assert.IsNotNull(player);

        player.transform.position = new Vector3(5f, 0f, 0f);

        var playerCol = player.GetComponent<Collider2D>();
        InvokeUnityMessage(dzComp, "OnTriggerEnter2D", playerCol);

        // Respawn ocurre tras 1.5s
        yield return new WaitForSeconds(1.6f);

        Assert.AreEqual(Vector2.zero, (Vector2)player.transform.position);
    }

    // helpers
    private static void InvokeUnityMessage(object target, string methodName, object arg)
    {
        var m = target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        Assert.IsNotNull(m, $"No existe {methodName} en {target.GetType().Name}");
        m.Invoke(target, new[] { arg });
    }

    private static Type FindTypeByName(string typeName)
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a =>
            {
                try { return a.GetTypes(); }
                catch { return Array.Empty<Type>(); }
            })
            .FirstOrDefault(t => t.Name == typeName);
    }
}
