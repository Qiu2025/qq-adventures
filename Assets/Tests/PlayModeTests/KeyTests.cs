using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class KeyTests
{
    private GameObject keyObject;
    private Key key;
    private GameObject player;
    private GameObject playerContainer;
    private Rigidbody2D rb;
    private readonly List<Object> toDestroy = new();

    [UnitySetUp]
    public IEnumerator UnitySetUp()
    {
        Physics2D.simulationMode = SimulationMode2D.FixedUpdate;
        Physics2D.gravity = Vector2.zero;

        // Player
        player = new GameObject("Player");
        toDestroy.Add(player);
        player.tag = "Player";
        var playerCol = player.AddComponent<BoxCollider2D>();
        playerCol.isTrigger = false;
        rb = player.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.gravityScale = 0f;
        player.transform.position = new Vector3(-2f, 0f, 0f);

        // PlayerContainer
        playerContainer = new GameObject("PlayerContainer");
        toDestroy.Add(playerContainer);
        playerContainer.tag = "PlayerContainer";

        // Key
        keyObject = new GameObject("Key");
        toDestroy.Add(keyObject);
        var keyCol = keyObject.AddComponent<CircleCollider2D>();
        keyCol.isTrigger = true;
        key = keyObject.AddComponent<Key>();
        keyObject.transform.position = Vector3.zero;

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
    public IEnumerator Key_attaches_to_playerContainer_and_shrinks()
    {
        Vector3 initialScale = keyObject.transform.localScale;

        rb.linearVelocity = new Vector2(10f, 0f);

        // Duración de la animación
        yield return SimulateForSeconds(0.5f);
        yield return null;

        var collider = keyObject.GetComponent<Collider2D>();
        Assert.IsFalse(collider.enabled, "El collider de la llave debería estar desactivado.");

        Assert.AreEqual(playerContainer.transform, keyObject.transform.parent,
            "La llave debería ser hija del PlayerContainer.");

        Assert.Less(keyObject.transform.localScale.magnitude, initialScale.magnitude,
            "La llave debería haberse reducido.");

        Assert.AreEqual(new Vector3(0f, -0.03f, 0f), keyObject.transform.localPosition,
            "El offset de la llave respecto al PlayerContainer debería ser (0, -0.03, 0).");
    }

    private IEnumerator SimulateForSeconds(float seconds)
    {
        var steps = Mathf.CeilToInt(seconds / Time.fixedDeltaTime);
        for (int i = 0; i < steps; i++)
            yield return new WaitForFixedUpdate();
    }
}
