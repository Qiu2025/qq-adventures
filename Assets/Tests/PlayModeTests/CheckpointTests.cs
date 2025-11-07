using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class CheckpointTests
{
    private GameObject checkpointObject;
    private Checkpoint checkpoint;
    private Animator animator;
    private GameObject player;
    private Rigidbody2D rb;
    private readonly List<Object> toDestroy = new();

    [UnitySetUp]
    public IEnumerator UnitySetUp()
    {
        Physics2D.simulationMode = SimulationMode2D.FixedUpdate;
        Physics2D.gravity = Vector2.zero;

        // Checkpoint
        checkpointObject = new GameObject("Checkpoint");
        toDestroy.Add(checkpointObject);
        var col = checkpointObject.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        animator = checkpointObject.AddComponent<Animator>();
        checkpoint = checkpointObject.AddComponent<Checkpoint>();
        checkpointObject.transform.position = Vector3.zero;

        // Player
        player = new GameObject("Player");
        toDestroy.Add(player);
        player.tag = "Player";
        var playerCol = player.AddComponent<BoxCollider2D>();
        playerCol.isTrigger = false;
        rb = player.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;
        player.transform.position = new Vector3(-2f, 0f, 0f);

        // GameManager
        var gm = new GameObject("GameManager");
        gm.AddComponent<GameManager>();
        toDestroy.Add(gm);

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
    public IEnumerator Checkpoint_activates_and_sets_position_in_GameManager()
    {
        rb.linearVelocity = new Vector2(10f, 0f);
        yield return SimulateForSeconds(0.4f); // tiempo para colisión y trigger
        yield return null;

        // Mover jugador a otro lugar y reaparecer
        player.transform.position = new Vector3(99f, 99f, 0f);
        GameManager.RespawnPlayer();

        Assert.AreEqual(Vector3.zero, player.transform.position, "El jugador debe reaparecer en la posición del checkpoint activado.");
        rb.linearVelocity = Vector3.zero;
    }

    private IEnumerator SimulateForSeconds(float seconds)
    {
        var steps = Mathf.CeilToInt(seconds / Time.fixedDeltaTime);
        for (int i = 0; i < steps; i++)
            yield return new WaitForFixedUpdate();
    }
}
