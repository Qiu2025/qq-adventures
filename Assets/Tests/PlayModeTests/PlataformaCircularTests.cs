using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
public class PlataformaCircularTests
{
    private GameObject plataforma;
    private GameObject jugador;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        // Plataforma
        plataforma = Object.Instantiate(Resources.Load<GameObject>("Plataforma_des_circular"));
        plataforma.transform.position = new Vector3(0f, 0f, 0f);

        // Jugador
        jugador = new GameObject("Player");
        jugador.tag = "Player"; 
        var rbJug = jugador.AddComponent<Rigidbody2D>();
        rbJug.bodyType = RigidbodyType2D.Dynamic;
        rbJug.gravityScale = 2f;
        var colJug = jugador.AddComponent<BoxCollider2D>();
        colJug.size = new Vector2(0.9f, 1.8f);

        // Ponemos al jugador encima de la plataforma a probar
        jugador.transform.position = new Vector3(0f, 2f, 0f);

        yield return new WaitForFixedUpdate();
        yield return null;
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        Object.Destroy(plataforma);
        Object.Destroy(jugador);
        yield return null;
    }

    [UnityTest]
    public IEnumerator PlayerSeHaceHijoAlColisionar_Y_SeDeshaceAlSalir()
    {
        // Esperamos a que el jugador aterrice y salte el OnCollisionEnter2D
        float timeout = 3f;
        while (timeout > 0f && jugador.transform.parent != plataforma.transform)
        {
            timeout -= Time.deltaTime;
            yield return null;
        }
        Assert.AreEqual(plataforma.transform, jugador.transform.parent, "El jugador debería hacerse hijo de la plataforma al contactar.");

        // Movemos al jugador fuera de la plataforma para forzar el OnCollisionExit2D
        jugador.transform.position += Vector3.right * 5f;
        yield return new WaitForFixedUpdate();
        yield return null;

        Assert.IsNull(jugador.transform.parent, "El jugador debería desparentarse al salir de la plataforma.");
    }
}
