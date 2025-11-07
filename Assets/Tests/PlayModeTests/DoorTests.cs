using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class DoorTests
{
    private GameObject doorObject;
    private Door door;
    private GameObject player;
    private Rigidbody2D rb;
    private readonly List<Object> toDestroy = new();

    [UnitySetUp]
    public IEnumerator UnitySetUp()
    {
        Physics2D.simulationMode = SimulationMode2D.FixedUpdate;
        Physics2D.gravity = Vector2.zero;

        // Door
        doorObject = new GameObject("Door");
        toDestroy.Add(doorObject);
        var doorCol = doorObject.AddComponent<BoxCollider2D>();
        doorCol.size = new Vector2(1.5f, 2f);
        door = doorObject.AddComponent<Door>();
        doorObject.transform.position = Vector3.zero;

        // Player
        player = new GameObject("Player");
        toDestroy.Add(player);
        player.tag = "Player";
        var playerCol = player.AddComponent<BoxCollider2D>();
        playerCol.size = new Vector2(1f, 1.8f);
        rb = player.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.gravityScale = 0f;

        player.transform.position = new Vector3(-3f, 0f, 0f);
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
    public IEnumerator Door_stays_active_without_key()
    {
        rb.linearVelocity = new Vector2(10f, 0f);
        yield return SimulateForSeconds(0.35f);
        yield return null;

        Assert.IsTrue(doorObject.activeSelf, "La puerta debe permanecer activa si el jugador no tiene llave");
        rb.linearVelocity = Vector2.zero;
    }

    [UnityTest]
    public IEnumerator Door_disables_and_destroys_key_with_key()
    {
        var keyGO = new GameObject("Key");
        toDestroy.Add(keyGO);
        keyGO.AddComponent<Key>();
        keyGO.transform.SetParent(player.transform, false);

        rb.linearVelocity = new Vector2(10f, 0f);
        yield return SimulateForSeconds(0.35f);
        yield return null;

        Assert.IsFalse(doorObject.activeSelf, "La puerta debe desactivarse y la llave destruirse si el jugador tiene llave");
        Assert.IsTrue(keyGO == null, "La llave debe ser destruida al abrir la puerta");
        rb.linearVelocity = Vector2.zero;
    }


    // Simula la física durante un número determinado de segundos
    private IEnumerator SimulateForSeconds(float seconds)
    {
        var steps = Mathf.CeilToInt(seconds / Time.fixedDeltaTime);
        for (int i = 0; i < steps; i++)
            yield return new WaitForFixedUpdate();
    }
}
