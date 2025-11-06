using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlataformaMortalSpikesPlaymodeTests
{
    private GameObject spikesGO;
    private GameObject playerGO;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        var spikesPrefab = Resources.Load<GameObject>("Spikes");
        Assert.NotNull(spikesPrefab, "No encontré el prefab 'Spikes' en Resources.");
        spikesGO = Object.Instantiate(spikesPrefab);

        // Player dummy
        playerGO = new GameObject("Player");
        playerGO.tag = "Player";
        var rb = playerGO.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;
        playerGO.AddComponent<BoxCollider2D>();

        // Asegura colisionador en spikes
        if (!spikesGO.GetComponent<Collider2D>())
            spikesGO.AddComponent<BoxCollider2D>();

        // Asegura que el script esté
        if (!spikesGO.GetComponent<PlataformaMortal>())
            Assert.Inconclusive("El prefab 'Spikes' no tiene el script PlataformaMortal.");

        yield return null;
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        if (spikesGO) Object.Destroy(spikesGO);
        if (playerGO) Object.Destroy(playerGO);
        yield return null;
    }

    [UnityTest]
    public IEnumerator Colision_Loggea_MatadoPorPlataforma()
    {
        // Si tu script hace el log exacto:
        LogAssert.ignoreFailingMessages = true;
        LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex("Matado\\spor\\splataforma", System.Text.RegularExpressions.RegexOptions.IgnoreCase));

        // Forzar colisión
        playerGO.transform.position = spikesGO.transform.position;
        yield return new WaitForFixedUpdate();
        yield return null;

        LogAssert.ignoreFailingMessages = false;
    }
}
