using UnityEngine;
using System.Collections;

// Script de las plataformas temporales
public class PlataformaTemporal : MonoBehaviour
{
    [SerializeField] public float tiempoEspera = 1.5f;  
    [SerializeField] public float tiempoRespawn = 3.0f;
    [SerializeField] public float despawnTime = 0.5f;
    [SerializeField] private Collider2D stepSensor; 

    private Rigidbody2D rb2D;
    private Collider2D col;
    private SpriteRenderer sr;

    private bool caida = false;
    private Vector3 startPos;
    private Quaternion startRot;

    // guardo el último collider del jugador para reactivar IgnoreCollision
    private Collider2D ultimoPlayerCol;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();

        startPos = transform.position;
        startRot = transform.rotation;

        rb2D.bodyType = RigidbodyType2D.Kinematic;
        rb2D.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    // Si el player toca la plataforma, ésta se cae
    public void ActivarPorPisada(Collider2D playerCol)
    {
        if (!caida)
            StartCoroutine(Caer(playerCol));
    }


    // La plataforma ingora al jugador y se cae
    private IEnumerator Caer(Collider2D playerCol)
    {
        caida = true;
        ultimoPlayerCol = playerCol;
        
        yield return StartCoroutine(Temblor(tiempoEspera, 0.05f)); // 0.05f = intensidad
        rb2D.constraints = RigidbodyConstraints2D.None;

        rb2D.bodyType = RigidbodyType2D.Dynamic;

        Physics2D.IgnoreCollision(col, playerCol, true);

        yield return new WaitForSeconds(despawnTime);
        StartCoroutine(Respawn());

    }

    // Se vuelve invisible durante un tiempo y vuelve al estado inicial.
    private IEnumerator Respawn()
    {
        sr.enabled = false;
        col.enabled = false;

        rb2D.linearVelocity = Vector2.zero;
        rb2D.angularVelocity = 0f;

        yield return new WaitForSeconds(tiempoRespawn);

        rb2D.constraints = RigidbodyConstraints2D.FreezeAll;

        transform.SetPositionAndRotation(startPos, startRot);

        if (ultimoPlayerCol != null)
            Physics2D.IgnoreCollision(col, ultimoPlayerCol, false);
        ultimoPlayerCol = null;

        sr.enabled = true;
        col.enabled = true;
        caida = false;
    }

    private IEnumerator Temblor(float duracion, float intensidad)
    {
        Vector3 posInicial = transform.localPosition;
        float t = 0f;

        while (t < duracion)
        {
            float x = Random.Range(-intensidad, intensidad);
            float y = Random.Range(-intensidad, intensidad);

            transform.localPosition = posInicial + new Vector3(x, y, 0);

            t += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = posInicial;
    }

}
