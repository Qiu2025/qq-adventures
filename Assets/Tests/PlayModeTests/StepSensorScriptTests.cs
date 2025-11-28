using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlataformaStepSensorTests
{
    private GameObject plataformaGO;
    private PlataformaTemporal plataforma;
    private GameObject sensorGO;
    private PlataformaStepSensor sensor;
    private GameObject player;
    private readonly List<Object> toDestroy = new();

    [UnitySetUp]
    public IEnumerator UnitySetUp()
    {
        // Plataforma con los componentes mínimos que espera PlataformaTemporal
        plataformaGO = new GameObject("PlataformaTemporal");
        toDestroy.Add(plataformaGO);
        plataformaGO.AddComponent<Rigidbody2D>();
        plataformaGO.AddComponent<BoxCollider2D>();
        plataformaGO.AddComponent<SpriteRenderer>();
        plataforma = plataformaGO.AddComponent<PlataformaTemporal>();

        // Sensor como hijo de la plataforma
        sensorGO = new GameObject("PlataformaStepSensor");
        toDestroy.Add(sensorGO);
        sensorGO.transform.SetParent(plataformaGO.transform);
        var trigger = sensorGO.AddComponent<BoxCollider2D>();
        trigger.isTrigger = true;
        sensor = sensorGO.AddComponent<PlataformaStepSensor>();

        // Player
        player = new GameObject("Player");
        toDestroy.Add(player);
        player.tag = "Player";
        player.AddComponent<BoxCollider2D>();

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
    public IEnumerator Sensor_triggers_plataforma_when_player_enters()
    {
        var flags = BindingFlags.Instance | BindingFlags.NonPublic;
        var fieldCaida = typeof(PlataformaTemporal).GetField("caida", flags);
        Assert.IsNotNull(fieldCaida, "No se encontró el campo privado 'caida' en PlataformaTemporal.");

        // Aseguramos estado inicial
        fieldCaida.SetValue(plataforma, false);

        var playerCol = player.GetComponent<Collider2D>();

        // Simular entrada del jugador
        sensor.SendMessage("OnTriggerEnter2D", playerCol);
        yield return null;

        bool caida = (bool)fieldCaida.GetValue(plataforma);
        Assert.IsTrue(caida, "La plataforma debe empezar la caída cuando el Player entra en el sensor.");
    }

    [UnityTest]
    public IEnumerator Sensor_does_nothing_when_non_player_enters()
    {
        var flags = BindingFlags.Instance | BindingFlags.NonPublic;
        var fieldCaida = typeof(PlataformaTemporal).GetField("caida", flags);
        Assert.IsNotNull(fieldCaida, "No se encontró el campo privado 'caida' en PlataformaTemporal.");

        fieldCaida.SetValue(plataforma, false);

        var other = new GameObject("Other");
        toDestroy.Add(other);
        other.tag = "Untagged";
        var otherCol = other.AddComponent<BoxCollider2D>();

        // Simular entrada de un objeto que no es Player
        sensor.SendMessage("OnTriggerEnter2D", otherCol);
        yield return null;

        bool caida = (bool)fieldCaida.GetValue(plataforma);
        Assert.IsFalse(caida, "La plataforma no debe activarse si el objeto no es el Player.");
    }
}
