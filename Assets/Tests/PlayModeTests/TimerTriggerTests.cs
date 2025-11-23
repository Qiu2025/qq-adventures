using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TimerTriggerTests
{
    private GameObject triggerObject;
    private TimerTrigger timerTrigger;
    private GameObject player;
    private BoxCollider2D triggerCollider;
    private GameObject temporizador;
    private GameObject tiempoUI;
    private readonly List<Object> toDestroy = new();

    [UnitySetUp]
    public IEnumerator UnitySetUp()
    {
        Physics2D.simulationMode = SimulationMode2D.FixedUpdate;
        Physics2D.gravity = Vector2.zero;

        // Trigger
        triggerObject = new GameObject("TimerTrigger");
        toDestroy.Add(triggerObject);
        triggerCollider = triggerObject.AddComponent<BoxCollider2D>();
        triggerCollider.isTrigger = true;
        timerTrigger = triggerObject.AddComponent<TimerTrigger>();

        // Temporizador y UI
        temporizador = new GameObject("Temporizador");
        tiempoUI = new GameObject("TiempoUI");
        toDestroy.Add(temporizador);
        toDestroy.Add(tiempoUI);
        temporizador.SetActive(false);
        tiempoUI.SetActive(false);

        // Asignar referencias serializadas
        typeof(TimerTrigger).GetField("temporizador", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(timerTrigger, temporizador);
        typeof(TimerTrigger).GetField("tiempoUI", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(timerTrigger, tiempoUI);
        typeof(TimerTrigger).GetField("triggerCollider", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(timerTrigger, triggerCollider);

        // Player
        player = new GameObject("Player");
        toDestroy.Add(player);
        player.tag = "Player";
        player.AddComponent<BoxCollider2D>();
        player.AddComponent<Rigidbody2D>().gravityScale = 0f;
        player.transform.position = Vector3.left * 2f;
        triggerObject.transform.position = Vector3.zero;

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
        // Se testea que el trigger activa temporizador y tiempoUI y desactiva el trigger
        //PlayerMovement.isFacingRight = true;

        // Simular OnTriggerExit2D
        timerTrigger.SendMessage("OnTriggerExit2D", player.GetComponent<Collider2D>());

        yield return null;

        Assert.IsTrue(temporizador.activeSelf, "El temporizador debe activarse");
        Assert.IsTrue(tiempoUI.activeSelf, "El tiempo UI debe activarse");
        Assert.IsFalse(triggerCollider.isTrigger, "El collider debe dejar de ser trigger");
    }

    [UnityTest]
    public IEnumerator TimerTrigger_does_nothing_if_player_not_facing_right()
    {
        // Se testea que no hace nada si el jugador no mira a la derecha
        //PlayerMovement.isFacingRight = false;

        timerTrigger.SendMessage("OnTriggerExit2D", player.GetComponent<Collider2D>());

        yield return null;

        Assert.IsFalse(temporizador.activeSelf, "El temporizador no debe activarse");
        Assert.IsFalse(tiempoUI.activeSelf, "El tiempo UI no debe activarse");
        Assert.IsTrue(triggerCollider.isTrigger, "El collider debe seguir siendo trigger");
    }
}
