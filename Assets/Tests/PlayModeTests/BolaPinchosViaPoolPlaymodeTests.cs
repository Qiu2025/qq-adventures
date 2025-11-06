using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BolaPinchosViaPoolPlaymodeTests
{
    private GameObject poolGO;
    private Camera cam;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        // Cámara simple por si la escena de test no tiene
        if (Camera.main == null)
        {
            var goCam = new GameObject("TestCamera");
            cam = goCam.AddComponent<Camera>();
            cam.orthographic = true;
            cam.transform.position = new Vector3(0, 0, -10);
        }

        // *** IMPORTANTE: Instanciar el POOL, no la bala ***
        var poolPrefab = Resources.Load<GameObject>("BulletPool");
        Assert.NotNull(poolPrefab, "No encontré el prefab 'BulletPool' en Resources.");
        poolGO = Object.Instantiate(poolPrefab);

        // Deja correr Awake/Start y espera a que se asigne Instance
        float wait = 0f;
        while (PoolBolas.Instance == null && wait < 1f)
        {
            wait += Time.deltaTime;
            yield return null;
        }
        Assert.IsNotNull(PoolBolas.Instance, "PoolBolas.Instance sigue siendo null tras instanciar 'BulletPool'.");

        // Si el prefabBola se asigna en Start, espera un frame extra
        yield return null;
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        if (poolGO) Object.Destroy(poolGO);
        if (cam) Object.Destroy(cam.gameObject);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ColisionConPlayer_DesactivaORecicla()
    {
        // Asegura pool listo
        var pool = PoolBolas.Instance;
        Assert.IsNotNull(pool, "PoolBolas.Instance es null.");
        Assert.IsNotNull(pool.prefabBola, "PoolBolas.prefabBola no está asignado.");

        var bola = pool.ObtenerBola();
        Assert.IsNotNull(bola, "ObtenerBola() devolvió null.");
        bola.transform.position = Vector3.zero;

        // Player dummy
        var player = new GameObject("Player");
        player.tag = "Player";
        var rbp = player.AddComponent<Rigidbody2D>();
        rbp.bodyType = RigidbodyType2D.Dynamic;
        rbp.gravityScale = 0f;
        player.AddComponent<BoxCollider2D>();
        player.transform.position = new Vector3(1.2f, 0f, 0f);

        // Disparo
        bola.SendMessage("Disparar", Vector2.right, SendMessageOptions.DontRequireReceiver);

        // Espera a que colisione / se recicle
        float timeout = 2f;
        while (timeout > 0f && bola.activeInHierarchy)
        {
            timeout -= Time.deltaTime;
            yield return null;
        }

        Assert.IsFalse(bola.activeSelf, "La bola debería desactivarse/reciclarse tras golpear al Player.");
        Object.Destroy(player);
    }
}
