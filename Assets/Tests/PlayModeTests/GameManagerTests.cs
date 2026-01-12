using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Reflection;

public class GameManagerTests
{
    private GameObject gmGO;
    private GameObject player;
    private GameObject uiCanvas;
    private GameObject chatObj;
    private GameObject powerEffect;
    private readonly List<Object> toDestroy = new();

    [UnitySetUp]
    public IEnumerator UnitySetUp()
    {
        // --- LIMPIEZA DE SINGLETON/ESTÁTICOS ENTRE TESTS ---
        if (GameManager.Instance != null)
        {
            Object.Destroy(GameManager.Instance.gameObject);
            // fuerza a null el backing field de la auto-property Instance
            typeof(GameManager)
                .GetField("<Instance>k__BackingField", BindingFlags.Static | BindingFlags.NonPublic)
                ?.SetValue(null, null);

            yield return null; // deja que Unity destruya el GO
        }

        GameManager.ResetRun();
        GameManager.SetGameOver(false);
        Time.timeScale = 1f;

        // --- tu setup actual ---
        Physics2D.simulationMode = SimulationMode2D.FixedUpdate;

        player = new GameObject("Player");
        toDestroy.Add(player);
        player.tag = "Player";
        player.AddComponent<BoxCollider2D>();
        player.transform.position = new Vector2(3f, 4f);

        uiCanvas = new GameObject("UICanvas");
        toDestroy.Add(uiCanvas);
        uiCanvas.tag = "UICanvas";
        uiCanvas.AddComponent<Animator>();

        chatObj = new GameObject("Chat");
        toDestroy.Add(chatObj);
        chatObj.tag = "Chat";
        var chatImage = new GameObject("ImagenMostrar");
        chatImage.transform.SetParent(chatObj.transform);
        chatImage.AddComponent<SpriteRenderer>();
        chatObj.SetActive(true);

        powerEffect = new GameObject("PowerEffect");
        toDestroy.Add(powerEffect);
        powerEffect.tag = "PowerEffect";
        powerEffect.AddComponent<Animator>();
        powerEffect.AddComponent<SpriteRenderer>();
        powerEffect.SetActive(true);

        gmGO = new GameObject("GameManager");
        toDestroy.Add(gmGO);
        gmGO.AddComponent<GameManager>();

        chatObj.SetActive(false);

        yield return null;
    }


    [UnityTearDown]
    public IEnumerator UnityTearDown()
    {
        GameManager.SetGameOver(false);
        Time.timeScale = 1f;

        foreach (var o in toDestroy)
            if (o) Object.Destroy(o);

        toDestroy.Clear();
        yield return null;
    }

    [UnityTest]
    public IEnumerator Singleton_instance_is_set_and_second_is_destroyed()
    {
        var firstInstance = GameManager.Instance;
        Assert.IsNotNull(firstInstance, "GameManager.Instance debe estar asignado tras el Awake.");

        var gm2 = new GameObject("GameManager2");
        toDestroy.Add(gm2);
        var comp2 = gm2.AddComponent<GameManager>();
        yield return null;

        Assert.AreSame(firstInstance, GameManager.Instance,
            "La segunda instancia no debe reemplazar a la primera.");
        Assert.IsTrue(comp2 == null,
            "La segunda instancia de GameManager debe destruirse en Awake.");
    }

    [UnityTest]
    public IEnumerator Respawn_without_checkpoint_uses_initial_player_position()
    {
        var startPos = (Vector2)player.transform.position;

        player.transform.position = new Vector2(99f, 99f);
        GameManager.RespawnPlayer();
        yield return null;

        Assert.AreEqual(startPos, (Vector2)player.transform.position,
            "Sin llamar a SetCheckpoint, el respawn debe ir a la posición inicial del jugador.");
    }

    [UnityTest]
    public IEnumerator SetCheckpoint_and_Respawn_moves_player_and_reenables_collider()
    {
        var target = new Vector2(-1.25f, 2.5f);
        GameManager.SetCheckpoint(target);

        var col = player.GetComponent<BoxCollider2D>();
        Assert.IsTrue(col.enabled, "El collider del jugador debe empezar habilitado.");

        player.transform.position = new Vector2(99f, 99f);
        GameManager.RespawnPlayer();
        yield return null;

        Assert.AreEqual(target, (Vector2)player.transform.position,
            "RespawnPlayer debe mover al jugador al último checkpoint guardado.");
        Assert.IsTrue(col.enabled, "Tras RespawnPlayer, el collider debe volver a habilitarse.");
        Assert.AreEqual(1f, Time.timeScale, 1e-4f,
            "RespawnPlayer debe restaurar Time.timeScale a 1.");
    }

    [UnityTest]
    public IEnumerator SetGameOver_true_and_false_controls_timescale()
    {
        GameManager.SetGameOver(true);
        yield return null;
        Assert.AreEqual(0f, Time.timeScale, 1e-4f,
            "SetGameOver(true) debe poner Time.timeScale a 0.");

        GameManager.SetGameOver(false);
        yield return null;
        Assert.AreEqual(1f, Time.timeScale, 1e-4f,
            "SetGameOver(false) debe restaurar Time.timeScale a 1.");
    }

    [UnityTest]
    public IEnumerator ShowChat_with_null_sprite_does_not_activate_chat()
    {
        chatObj.SetActive(false);

        GameManager.ShowChat(null, 0.1f, 0.1f, 0.1f);
        yield return null;

        Assert.IsFalse(chatObj.activeSelf,
            "Si el sprite es null, ShowChat no debe activar el chat.");
    }

[UnityTest]
public IEnumerator ShowChat_shows_then_hides_chat()
{
    chatObj.SetActive(false);

    var tex = new Texture2D(2, 2);
    var sprite = Sprite.Create(tex, new Rect(0, 0, 2, 2), new Vector2(0.5f, 0.5f));

    GameManager.ShowChat(sprite, 0f, 0f, 0f);

    Assert.IsTrue(chatObj.activeSelf, "El chat debe activarse al comenzar ShowChatRoutine.");

    yield return null;

    Assert.IsFalse(chatObj.activeSelf, "El chat debe desactivarse al terminar ShowChatRoutine.");
}


    [UnityTest]
    public IEnumerator PowerEffect_enables_then_disables_effect()
    {
        powerEffect.SetActive(false);

        GameManager.PowerEffect();
        Assert.IsTrue(powerEffect.activeSelf, "PowerEffect debe activar el objeto de efecto.");
        
        yield return null;
        Assert.IsFalse(powerEffect.activeSelf, "Al finalizar la corutina, el efecto debe desactivarse.");
    }
}
