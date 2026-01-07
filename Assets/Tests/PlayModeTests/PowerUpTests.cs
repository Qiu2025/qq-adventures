using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class DashPowerUpTests
{
    private GameObject dashObject;
    private DashPowerUp dashPowerUp;
    private SpriteRenderer sr;
    private CircleCollider2D col;
    private readonly List<Object> toDestroy = new();

    [UnitySetUp]
    public IEnumerator UnitySetUp()
    {
        // PowerUp
        dashObject = new GameObject("DashPowerUp");
        toDestroy.Add(dashObject);
        sr = dashObject.AddComponent<SpriteRenderer>();
        col = dashObject.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        dashPowerUp = dashObject.AddComponent<DashPowerUp>();

        yield return null;
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
    public IEnumerator DashPowerUp_does_nothing_when_non_player_enters()
    {
        var other = new GameObject("Other");
        toDestroy.Add(other);
        var otherCol = other.AddComponent<BoxCollider2D>();
        other.tag = "Untagged";

        bool initialSr = sr.enabled;
        bool initialCol = col.enabled;

        dashPowerUp.SendMessage("OnTriggerEnter2D", otherCol);
        yield return null;

        Assert.AreEqual(initialSr, sr.enabled, "El SpriteRenderer no debe cambiar si no entra el Player.");
        Assert.AreEqual(initialCol, col.enabled, "El Collider no debe cambiar si no entra el Player.");
    }

    [UnityTest]
    public IEnumerator DashPowerUp_respawns_after_delay()
    {
        Assert.IsTrue(sr.enabled, "El SpriteRenderer debe empezar habilitado.");
        Assert.IsTrue(col.enabled, "El Collider debe empezar habilitado.");

        // Lanzamos directamente la corrutina de respawn
        dashPowerUp.StartCoroutine("RespawnRoutine");

        // Primer frame: desactivado
        yield return null;
        Assert.IsFalse(sr.enabled, "El SpriteRenderer debe desactivarse al empezar el respawn.");
        Assert.IsFalse(col.enabled, "El Collider debe desactivarse al empezar el respawn.");

        // Esperar más que el tiempo de respawn
        yield return new WaitForSeconds(2.1f);

        Assert.IsTrue(sr.enabled, "El SpriteRenderer debe reactivarse tras el respawn.");
        Assert.IsTrue(col.enabled, "El Collider debe reactivarse tras el respawn.");
    }
}
