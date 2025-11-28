using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TimerTriggerTests
{
    private GameObject triggerObject;
    private TimerTrigger timerTrigger;
    private GameObject player;
    private PlayerMovement playerMovement;
    private BoxCollider2D triggerCollider;
    private GameObject temporizador;
    private GameObject tiempoUI;
    private readonly List<Object> toDestroy = new();

    [UnitySetUp]
    public IEnumerator UnitySetUp()
    {
        Physics2D.simulationMode = SimulationMode2D.FixedUpdate;
        Physics2D.gravity = Vector2.zero;

        player = new GameObject("Player");
        toDestroy.Add(player);
        player.tag = "Player";
        player.AddComponent<BoxCollider2D>();
        player.AddComponent<Rigidbody2D>().gravityScale = 0f;
        playerMovement = player.AddComponent<PlayerMovement>();
        playerMovement.enabled = false; // evitar que Start() se ejecute
        player.transform.position = Vector3.left * 2f;

        temporizador = new GameObject("Temporizador");
        tiempoUI = new GameObject("TiempoUI");
        toDestroy.Add(temporizador);
        toDestroy.Add(tiempoUI);
        temporizador.SetActive(false);
        tiempoUI.SetActive(false);

        triggerObject = new GameObject("TimerTrigger");
        toDestroy.Add(triggerObject);
        triggerCollider = triggerObject.AddComponent<BoxCollider2D>();
        triggerCollider.isTrigger = true;
        triggerObject.transform.position = Vector3.zero;
        timerTrigger = triggerObject.AddComponent<TimerTrigger>();

        var ttType = typeof(TimerTrigger);
        ttType.GetField("temporizador", BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(timerTrigger, temporizador);
        ttType.GetField("tiempoUI", BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(timerTrigger, tiempoUI);
        ttType.GetField("triggerCollider", BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(timerTrigger, triggerCollider);

        yield return new WaitForFixedUpdate();
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
    public IEnumerator TimerTrigger_toggles_objects_and_disables_trigger_when_player_exits_right()
    {
        SetFacingRight(true);

        timerTrigger.SendMessage("OnTriggerExit2D", player.GetComponent<Collider2D>());
        yield return null;

        Assert.IsTrue(temporizador.activeSelf, "El temporizador debe activarse");
        Assert.IsTrue(tiempoUI.activeSelf, "El tiempo UI debe activarse");
        Assert.IsFalse(triggerCollider.isTrigger, "El collider debe dejar de ser trigger");
    }

    [UnityTest]
    public IEnumerator TimerTrigger_does_nothing_if_player_not_facing_right()
    {
        SetFacingRight(false);

        timerTrigger.SendMessage("OnTriggerExit2D", player.GetComponent<Collider2D>());
        yield return null;

        Assert.IsFalse(temporizador.activeSelf, "El temporizador no debe activarse");
        Assert.IsFalse(tiempoUI.activeSelf, "El tiempo UI no debe activarse");
        Assert.IsTrue(triggerCollider.isTrigger, "El collider debe seguir siendo trigger");
    }

    [UnityTest]
    public IEnumerator TimerTrigger_does_nothing_if_non_player_exits()
    {
        SetFacingRight(true);

        var other = new GameObject("Other");
        toDestroy.Add(other);
        var otherCol = other.AddComponent<BoxCollider2D>();
        other.tag = "Untagged";

        timerTrigger.SendMessage("OnTriggerExit2D", otherCol);
        yield return null;

        Assert.IsFalse(temporizador.activeSelf, "El temporizador no debe activarse con otros objetos");
        Assert.IsFalse(tiempoUI.activeSelf, "El tiempo UI no debe activarse con otros objetos");
        Assert.IsTrue(triggerCollider.isTrigger, "El collider debe seguir siendo trigger");
    }

    private void SetFacingRight(bool value)
    {
        var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        var type = typeof(PlayerMovement);

        var field = type.GetField("isFacingRight", flags);
        if (field != null)
        {
            field.SetValue(playerMovement, value);
            return;
        }

        var prop = type.GetProperty("isFacingRight", flags);
        if (prop != null)
        {
            prop.SetValue(playerMovement, value);
        }
        else
        {
            Assert.Fail("PlayerMovement no tiene campo ni propiedad 'isFacingRight'.");
        }
    }
}
