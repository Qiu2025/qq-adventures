using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class GameManagerTests
{
    private GameObject gmGO;
    private GameObject player;
    private readonly List<Object> toDestroy = new();

    [UnitySetUp]
    public IEnumerator UnitySetUp()
    {
        Physics2D.simulationMode = SimulationMode2D.FixedUpdate;
        Time.timeScale = 1f; // asegurar tiempo normal al inicio

        player = new GameObject("Player");
        toDestroy.Add(player);
        player.tag = "Player";
        player.AddComponent<BoxCollider2D>();
        player.transform.position = new Vector2(3f, 4f);

        gmGO = new GameObject("GameManager");
        toDestroy.Add(gmGO);
        gmGO.AddComponent<GameManager>();

        yield return null;
    }

    [UnityTearDown]
    public IEnumerator UnityTearDown()
    {
        // restaurar estado global para no bloquear tests siguientes
        GameManager.SetGameOver(false);
        Time.timeScale = 1f;

        foreach (var o in toDestroy) if (o) Object.Destroy(o);
        toDestroy.Clear();
        yield return null;
    }

    [UnityTest]
    public IEnumerator SetCheckpoint_and_Respawn_moves_player_and_reenables_collider()
    {
        // Se testea que SetCheckpoint guarda posición y Respawn reposiciona y mantiene collider habilitado
        var target = new Vector2(-1.25f, 2.5f);
        GameManager.SetCheckpoint(target);

        var col = player.GetComponent<BoxCollider2D>();
        Assert.IsTrue(col.enabled);

        player.transform.position = new Vector2(99, 99);
        GameManager.RespawnPlayer();
        yield return null;

        Assert.AreEqual(target, (Vector2)player.transform.position);
        Assert.IsTrue(col.enabled);
        Assert.AreEqual(1f, Time.timeScale, 1e-4f);
    }

    [UnityTest]
    public IEnumerator SetGameOver_sets_timescale_zero()
    {
        // Se testea que SetGameOver pausa el juego
        GameManager.SetGameOver(true);
        yield return null;
        Assert.AreEqual(0f, Time.timeScale, 1e-4f);
    }
}
