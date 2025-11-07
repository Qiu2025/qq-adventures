using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlataformaHorizontalTests
{
    private GameObject plataforma;
    private GameObject jugador;
    private BoxCollider2D colPlat;
    private readonly List<Object> toDestroy = new();

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        // Físicas deterministas en FixedUpdate
        Physics2D.simulationMode = SimulationMode2D.FixedUpdate;
        Physics2D.gravity = new Vector2(0f, -9.81f);

        // --- Plataforma desde Resources o fallback ---
        plataforma = Object.Instantiate(Resources.Load<GameObject>("Plataforma"));
        if (plataforma == null)
        {
            plataforma = new GameObject("Plataforma (fallback)");
        }
        toDestroy.Add(plataforma);
        plataforma.transform.position = Vector3.zero;

        // Asegurar componentes mínimos
        colPlat = plataforma.GetComponent<BoxCollider2D>();
        if (colPlat == null)
        {
            colPlat = plataforma.AddComponent<BoxCollider2D>();
            colPlat.size = new Vector2(3f, 0.5f);
        }

        var rbPlat = plataforma.GetComponent<Rigidbody2D>();
        if (rbPlat == null) rbPlat = plataforma.AddComponent<Rigidbody2D>();
        rbPlat.bodyType = RigidbodyType2D.Kinematic;   // la mueve el script

        if (plataforma.GetComponent<PlataformaHorizontal>() == null)
        {
            var ph = plataforma.AddComponent<PlataformaHorizontal>();
            ph.velocidad = 2f;
            ph.distanciaMaxima = 2f;
        }

        // --- Jugador ---
        jugador = new GameObject("Player");
        toDestroy.Add(jugador);
        jugador.tag = "Player";
        var rbJug = jugador.AddComponent<Rigidbody2D>();
        rbJug.bodyType = RigidbodyType2D.Dynamic;
        rbJug.gravityScale = 2f;
        rbJug.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        var colJug = jugador.AddComponent<BoxCollider2D>();
        colJug.size = new Vector2(0.9f, 1.8f);

        // Colocar justo encima tocando ligeramente
        float platTopY = plataforma.transform.position.y + (colPlat.size.y * 0.5f);
        float playerHalfY = colJug.size.y * 0.5f;
        jugador.transform.position = new Vector3(0f, platTopY + playerHalfY + 0.02f, 0f);

        // Empuje hacia abajo para asegurar el contacto
        rbJug.linearVelocity = new Vector2(0f, -2f);

        // Deja correr Start/FixedUpdate iniciales
        yield return new WaitForFixedUpdate();
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        foreach (var o in toDestroy)
            if (o != null) Object.Destroy(o);
        toDestroy.Clear();
        yield return null;
    }

    [UnityTest]
    public IEnumerator PlayerSeHaceHijoAlColisionar_Y_SeDeshaceAlSalir()
    {
        // Esperar hasta 2s en pasos de física a que se parentice
        bool parentado = false;
        int enterSteps = Mathf.CeilToInt(2f / Time.fixedDeltaTime);
        for (int i = 0; i < enterSteps; i++)
        {
            yield return new WaitForFixedUpdate();
            if (jugador.transform.parent == plataforma.transform)
            {
                parentado = true;
                break;
            }
        }
        Assert.IsTrue(parentado, "El jugador debería hacerse hijo de la plataforma al contactar.");

        // Mover al jugador fuera del contacto y esperar desparentado
        float extra = Mathf.Max(2f, colPlat.size.x + 1f);
        jugador.transform.position += Vector3.right * extra;

        var rbJug = jugador.GetComponent<Rigidbody2D>();
        if (rbJug != null) rbJug.linearVelocity = Vector2.zero;

        int exitSteps = Mathf.CeilToInt(1f / Time.fixedDeltaTime);
        for (int i = 0; i < exitSteps; i++)
        {
            yield return new WaitForFixedUpdate();
            if (jugador.transform.parent == null)
                break;
        }

        Assert.IsNull(jugador.transform.parent, "El jugador debería desparentarse al salir de la plataforma.");
    }

    // Helper: simula física durante 'seconds'
    private IEnumerator SimulateForSeconds(float seconds)
    {
        int steps = Mathf.CeilToInt(seconds / Time.fixedDeltaTime);
        for (int i = 0; i < steps; i++)
            yield return new WaitForFixedUpdate();
    }
}
