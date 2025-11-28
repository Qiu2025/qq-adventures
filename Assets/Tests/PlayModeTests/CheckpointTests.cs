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
    private GameObject gmPrefab;

    [UnitySetUp]
    public IEnumerator UnitySetUp()
    {
        Physics2D.simulationMode = SimulationMode2D.FixedUpdate;
        Physics2D.gravity = Vector2.zero;

        gmPrefab = Resources.Load<GameObject>("Prefabs/GameManager");

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
        
        // UICanvas con Animator
        var uiCanvas = new GameObject("UICanvas");
        uiCanvas.tag = "UICanvas";
        uiCanvas.AddComponent<Animator>();
        toDestroy.Add(uiCanvas);

        // ShowChat
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

        Assert.AreEqual(Vector3.zero, player.transform.position,
            "El jugador debe reaparecer en la posición del checkpoint activado.");
        rb.linearVelocity = Vector3.zero;
    }

    // Nuevo test: sin pasar por checkpoint, debe reaparecer en la posición inicial
    [UnityTest]
    public IEnumerator Respawn_without_checkpoint_uses_initial_player_position()
    {
        var initialPosition = player.transform.position;

        // Mover jugador lejos
        player.transform.position = new Vector3(50f, 50f, 0f);

        // Respawn sin haber tocado ningún checkpoint
        GameManager.RespawnPlayer();

        Assert.AreEqual(initialPosition, player.transform.position,
            "Sin pasar por un checkpoint, el jugador debe reaparecer en su posición inicial.");
        yield return null;
    }

    // Nuevo test: el checkpoint sólo debe activarse una vez
    [UnityTest]
    public IEnumerator Checkpoint_only_activates_once()
    {
        // Primera pasada: checkpoint en (0,0)
        rb.linearVelocity = new Vector2(10f, 0f);
        yield return SimulateForSeconds(0.4f);
        yield return null;

        var firstCheckpointPos = checkpointObject.transform.position; // debería ser (0,0)

        // Alejar al jugador del primer checkpoint
        rb.linearVelocity = Vector2.zero;
        player.transform.position = new Vector3(-2f, 2f, 0f);

        // Mover el checkpoint a otra posición
        var newCheckpointPos = new Vector3(5f, 0f, 0f);
        checkpointObject.transform.position = newCheckpointPos;

        // Hacer que el jugador pase de nuevo por donde está ahora el checkpoint
        player.transform.position = new Vector3(3f, 0f, 0f);
        rb.linearVelocity = new Vector2(10f, 0f);
        yield return SimulateForSeconds(0.4f);
        yield return null;

        // Respawn después de mover al jugador a otra posición
        rb.linearVelocity = Vector2.zero;
        player.transform.position = new Vector3(99f, 99f, 0f);
        GameManager.RespawnPlayer();

        Assert.AreEqual(firstCheckpointPos, player.transform.position,
            "El checkpoint solo debe activarse una vez; colisiones posteriores no deben cambiar la posición de respawn.");
        Assert.AreNotEqual(newCheckpointPos, player.transform.position,
            "El respawn no debe usar la nueva posición del checkpoint tras intentar reactivarlo.");
    }

    private IEnumerator SimulateForSeconds(float seconds)
    {
        var steps = Mathf.CeilToInt(seconds / Time.fixedDeltaTime);
        for (int i = 0; i < steps; i++)
            yield return new WaitForFixedUpdate();
    }
}
