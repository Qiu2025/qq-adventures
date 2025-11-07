using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.Tilemaps;

public class HiddenZoneTests
{
    private GameObject hiddenZoneObject;
    private HiddenZone hiddenZone;
    private GameObject player;
    private readonly List<Object> toDestroy = new();

    [UnitySetUp]
    public IEnumerator UnitySetUp()
    {
        Physics2D.simulationMode = SimulationMode2D.FixedUpdate;
        Physics2D.gravity = Vector2.zero;

        // Hidden Zone
        hiddenZoneObject = new GameObject("HiddenZone");
        toDestroy.Add(hiddenZoneObject);
        var zoneCol = hiddenZoneObject.AddComponent<BoxCollider2D>();
        zoneCol.isTrigger = true;
        var tilemapRenderer = hiddenZoneObject.AddComponent<TilemapRenderer>();
        tilemapRenderer.material = new Material(Shader.Find("Sprites/Default"));
        hiddenZone = hiddenZoneObject.AddComponent<HiddenZone>();
        hiddenZoneObject.transform.position = Vector3.zero;

        // Player
        player = new GameObject("Player");
        toDestroy.Add(player);
        player.tag = "Player";
        var playerCol = player.AddComponent<BoxCollider2D>();
        playerCol.isTrigger = false;
        var rb = player.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;
        player.transform.position = new Vector3(-2f, 0f, 0f);

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
    public IEnumerator HiddenZone_fades_out_on_trigger_enter()
    {
        var rb = player.GetComponent<Rigidbody2D>();
        Color initialColor = hiddenZoneObject.GetComponent<TilemapRenderer>().material.color;

        rb.linearVelocity = new Vector2(10f, 0f);
        yield return new WaitForFixedUpdate();
        yield return new WaitForSeconds(0.2f);

        Color finalColor = hiddenZoneObject.GetComponent<TilemapRenderer>().material.color;
        Assert.Less(finalColor.a, initialColor.a, "El alpha debe disminuir al entrar el jugador (fade out)");
    }

    [UnityTest]
    public IEnumerator HiddenZone_fades_in_on_trigger_exit()
    {
        var rb = player.GetComponent<Rigidbody2D>();
        var tilemap = hiddenZoneObject.GetComponent<TilemapRenderer>();
        tilemap.material.color = new Color(1f, 1f, 1f, 0f);

        // Entrar y luego salir
        rb.linearVelocity = new Vector2(10f, 0f);
        yield return new WaitForFixedUpdate();
        yield return new WaitForSeconds(0.2f);

        rb.linearVelocity = new Vector2(-10f, 0f);
        yield return new WaitForFixedUpdate();
        yield return new WaitForSeconds(0.2f);

        Color finalColor = tilemap.material.color;
        Assert.Greater(finalColor.a, 0f, "El alpha debe aumentar al salir el jugador (fade in)");
    }
}
