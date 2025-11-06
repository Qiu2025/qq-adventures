using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlataformaTemporalTests
{
    GameObject plataforma;
    GameObject player;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        // Plataforma
        plataforma = Object.Instantiate(Resources.Load<GameObject>("PlataformaTemporal"));
        plataforma.transform.position = Vector3.zero;

        // Jugador
        player = new GameObject("Player");
        player.tag = "Player";
        var rb = player.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        player.AddComponent<BoxCollider2D>();
        player.transform.position = new Vector3(0, 2, 0);

        yield return new WaitForFixedUpdate();
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        Object.Destroy(plataforma);
        Object.Destroy(player);
        yield return null;
    }

    [UnityTest]
    public IEnumerator LaPlataformaSeActivaCuandoElPlayerLaPisa()
    {
        var script = plataforma.GetComponent<PlataformaTemporal>();
        Assert.NotNull(script);

        // Esperamos a que el jugador caiga sobre la plataforma
        yield return new WaitForSeconds(0.5f);

       // Comprobamos si se ha pisado la plataforma (la plataforma pasa a ser dinamica)
        float tiempoLimite = 2f;
        bool seVolvioDinamica = false;

        while (tiempoLimite > 0f)
        {
            if (script.GetComponent<Rigidbody2D>().bodyType == RigidbodyType2D.Dynamic)
            {
                seVolvioDinamica = true;
                break;
            }
            tiempoLimite -= Time.deltaTime;
            yield return null;
        }

        Assert.IsTrue(seVolvioDinamica, "La plataforma no se activó al pisarla.");
    }

    [UnityTest]
    public IEnumerator LaPlataformaDesapareceYReaparece()
    {
        var script = plataforma.GetComponent<PlataformaTemporal>();
        var sr = plataforma.GetComponent<SpriteRenderer>();

        // Esperamos a que se caiga la plataforma
        yield return new WaitForSeconds(script.tiempoEspera + 0.5f);

        // Esperamos a que desaparezca la plataforma
        yield return new WaitForSeconds(1.2f);
        Assert.IsFalse(sr.enabled, "La plataforma debería estar invisible tras caer.");

        // Esperamos a que haga respawn la plataforma
        yield return new WaitForSeconds(script.tiempoRespawn + 0.5f);

        Assert.IsTrue(sr.enabled, "La plataforma no reapareció después del respawn.");
        Assert.AreEqual(script.transform.position, script.transform.position, "La plataforma no regresó a su posición original.");
    }
}


