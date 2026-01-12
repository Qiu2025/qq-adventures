using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerFollowContainerTests
{
    private readonly List<Object> toDestroy = new();
    private GameObject player;
    private GameObject container;

    [UnitySetUp]
    public IEnumerator UnitySetUp()
    {
        player = new GameObject("Player");
        toDestroy.Add(player);
        player.tag = "Player";
        player.transform.position = new Vector3(5f, 2f, 0f);

        container = new GameObject("Container");
        toDestroy.Add(container);
        container.AddComponent<PlayerFollowContainer>();

        // 1 frame para que corra Start, 1 frame extra para asegurar Update
        yield return null;
        yield return null;
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
    public IEnumerator Start_busca_al_jugador_y_alinea_posicion()
    {
        Assert.AreEqual(player.transform.position, container.transform.position,
            "Tras Start/primer Update, debe colocarse en la posición del jugador.");
        yield break;
    }

    [UnityTest]
    public IEnumerator Update_sigue_al_jugador_cada_frame()
    {
        player.transform.position = new Vector3(8f, -3f, 0f);

        // Espera un frame para que Update copie la nueva posición
        yield return null;

        Assert.AreEqual(player.transform.position, container.transform.position,
            "Debe actualizar su posición para seguir al jugador.");
    }
}