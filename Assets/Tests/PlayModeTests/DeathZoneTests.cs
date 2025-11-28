using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class DeathZoneTests
{
    private GameObject deathZoneObject;
    private GameObject player;
    private Rigidbody2D rb;
    private GameObject gm;
    private readonly List<Object> toDestroy = new();

    [UnitySetUp]
    public IEnumerator UnitySetUp()
    {
        Physics2D.simulationMode = SimulationMode2D.FixedUpdate;
        Physics2D.gravity = Vector2.zero;

        // DeathZone
        deathZoneObject = new GameObject("DeathZone");
        toDestroy.Add(deathZoneObject);
        var dzCol = deathZoneObject.AddComponent<BoxCollider2D>();
        dzCol.isTrigger = true;
        deathZoneObject.AddComponent<DeathZone>();
        deathZoneObject.transform.position = Vector3.zero;

        // Player
        player = new GameObject("Player");
        toDestroy.Add(player);
        player.tag = "Player";
        var playerCol = player.AddComponent<BoxCollider2D>();
        playerCol.isTrigger = false;
        rb = player.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;
        player.transform.position = new Vector3(-3f, 0f, 0f);

        // UICanvas
        var uiCanvas = new GameObject("UICanvas");
        uiCanvas.tag = "UICanvas";
        uiCanvas.AddComponent<Animator>();
        toDestroy.Add(uiCanvas);

        // Chat
        var chatObj = new GameObject("Chat");
        chatObj.tag = "Chat";
        var chatImage = new GameObject("ImagenMostrar");
        chatImage.transform.SetParent(chatObj.transform);
        chatImage.AddComponent<SpriteRenderer>();
        toDestroy.Add(chatObj);

        // PowerEffect
        var powerEffect = new GameObject("PowerEffect");
        powerEffect.tag = "PowerEffect";
        powerEffect.AddComponent<Animator>();
        powerEffect.AddComponent<SpriteRenderer>();
        toDestroy.Add(powerEffect);

        // GameManager
        gm = new GameObject("GameManager");
        gm.AddComponent<GameManager>();
        toDestroy.Add(gm);

        GameManager.SetCheckpoint(Vector2.zero);

        yield return new WaitForFixedUpdate();
    }

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
    public IEnumerator Player_respawns_when_enters_deathzone()
    {
        player.transform.position = new Vector3(-3f, 0f, 0f);
        rb.linearVelocity = new Vector2(10f, 0f);

        yield return SimulateForSeconds(0.4f);
        yield return null;

        Assert.AreEqual(Vector2.zero, (Vector2)player.transform.position, "El jugador debe reaparecer en el último checkpoint al entrar en la DeathZone.");
        rb.linearVelocity = Vector2.zero;
    }

    private IEnumerator SimulateForSeconds(float seconds)
    {
        var steps = Mathf.CeilToInt(seconds / Time.fixedDeltaTime);
        for (int i = 0; i < steps; i++)
            yield return new WaitForFixedUpdate();
    }
}
