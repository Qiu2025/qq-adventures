using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class PlayerMovementTests
{
    private GameObject player;
    private PlayerMovement playerMovement;
    private Temporizador temporizador;
    private Slider slider;
    private readonly List<Object> toDestroy = new();

    [UnitySetUp]
    public IEnumerator UnitySetUp()
    {
        // Temporizador de prueba
        var timerGO = new GameObject("Temporizador");
        toDestroy.Add(timerGO);
        temporizador = timerGO.AddComponent<Temporizador>();

        var sliderGO = new GameObject("Slider");
        toDestroy.Add(sliderGO);
        slider = sliderGO.AddComponent<Slider>();

        var imgGO = new GameObject("Relleno");
        toDestroy.Add(imgGO);
        var relleno = imgGO.AddComponent<Image>();

        temporizador.sliderTemporizador = slider;
        temporizador.relleno = relleno;
        temporizador.tiempoMaximo = 20f;
        temporizador.ActivarTemporizador();

        // Player + PlayerMovement (deshabilitado para que no corra Start/Update)
        player = new GameObject("Player");
        toDestroy.Add(player);
        player.tag = "Player";
        player.AddComponent<Rigidbody2D>();
        player.AddComponent<SpriteRenderer>();
        player.AddComponent<TrailRenderer>();
        var anim = player.AddComponent<Animator>();

        playerMovement = player.AddComponent<PlayerMovement>();
        playerMovement.enabled = false;

        // Asignar campos serializados que vamos a usar
        var type = typeof(PlayerMovement);
        type.GetField("rb", BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(playerMovement, player.GetComponent<Rigidbody2D>());
        type.GetField("sr", BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(playerMovement, player.GetComponent<SpriteRenderer>());
        type.GetField("animator", BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(playerMovement, anim);
        type.GetField("tr", BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(playerMovement, player.GetComponent<TrailRenderer>());

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
    public IEnumerator PowerUp_sets_dash_and_jump_state()
    {
        var type = typeof(PlayerMovement);
        type.GetField("jumpingPower", BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(playerMovement, 12f);
        type.GetField("secondJumpingPower", BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(playerMovement, 5f);
        type.GetField("usedJumps", BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(playerMovement, 0);

        playerMovement.canDash = false;
        playerMovement.dashedInAir = true;

        playerMovement.PowerUp();

        var usedJumps = (int)type.GetField("usedJumps",
            BindingFlags.NonPublic | BindingFlags.Instance).GetValue(playerMovement);
        var jumpingPower = (float)type.GetField("jumpingPower",
            BindingFlags.NonPublic | BindingFlags.Instance).GetValue(playerMovement);
        var secondJumpingPower = (float)type.GetField("secondJumpingPower",
            BindingFlags.NonPublic | BindingFlags.Instance).GetValue(playerMovement);

        Assert.IsTrue(playerMovement.canDash, "PowerUp debe permitir hacer dash.");
        Assert.IsFalse(playerMovement.dashedInAir, "PowerUp debe resetear dashedInAir a false.");
        Assert.AreEqual(1, usedJumps, "PowerUp debe dejar usedJumps en 1.");
        Assert.AreEqual(jumpingPower, secondJumpingPower, 1e-4f,
            "PowerUp debe igualar secondJumpingPower a jumpingPower.");

        yield break;
    }

    [UnityTest]
    public IEnumerator OnTriggerEnter_IceCream_increases_timer_and_disables_icecream()
    {
        var initialTime = slider.value;

        var iceCream = new GameObject("IceCream");
        toDestroy.Add(iceCream);
        iceCream.tag = "IceCream";
        var sr = iceCream.AddComponent<SpriteRenderer>();
        var col = iceCream.AddComponent<BoxCollider2D>();

        var method = typeof(PlayerMovement).GetMethod("OnTriggerEnter2D",
            BindingFlags.NonPublic | BindingFlags.Instance);

        method.Invoke(playerMovement, new object[] { col });

        Assert.Greater(slider.value, initialTime,
            "Al recoger un helado, el temporizador debe aumentar.");
        Assert.IsFalse(sr.enabled, "El sprite del helado debe desactivarse tras recogerlo.");
        Assert.IsFalse(col.enabled, "El collider del helado debe desactivarse tras recogerlo.");

        yield break;
    }

    [UnityTest]
    public IEnumerator CheckFlip_changes_facing_direction()
    {
        var type = typeof(PlayerMovement);

        playerMovement.isFacingRight = true;
        type.GetField("horizontal", BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(playerMovement, -1f);

        var method = typeof(PlayerMovement).GetMethod("CheckFlip",
            BindingFlags.NonPublic | BindingFlags.Instance);

        method.Invoke(playerMovement, null);

        Assert.IsFalse(playerMovement.isFacingRight,
            "Al moverse a la izquierda, isFacingRight debe pasar a false.");

        yield break;
    }
}
