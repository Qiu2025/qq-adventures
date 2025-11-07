using UnityEngine;

public class CactusDispara : MonoBehaviour
{
    [Header("Disparo")]
    public float tiempoEntreDisparos = 2f;
    public Vector2 direccionDisparo = Vector2.left;
    public Transform puntoDisparo; // opcional (FirePoint). Si no se asigna, usa centro del sprite/collider.

    private float temporizador;
    private SpriteRenderer sr;
    private Collider2D col;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }
    void Start()
    {
        // TEST: dispara una vez al empezar
        GameObject bola = PoolBolas.Instance != null ? PoolBolas.Instance.ObtenerBola() : null;
        if (bola == null)
        {
            Debug.LogError("No hay PoolBolas en escena o no hay prefab asignado.");
            return;
        }

        // Si no hay FirePoint asignado, calculará el centro
        Vector3 desde = ObtenerPosicionDisparo();
        bola.transform.position = desde;
        bola.transform.rotation = Quaternion.identity;

        var script = bola.GetComponent<BolaPinchos>();
        if (script == null)
        {
            Debug.LogError("El prefab no tiene BolaPinchos.cs");
            return;
        }

        script.Disparar(direccionDisparo == Vector2.zero ? Vector2.left : direccionDisparo);
        Debug.Log("Disparo de prueba ejecutado.");
    }

    void Update()
    {
        temporizador += Time.deltaTime;
        if (temporizador >= tiempoEntreDisparos)
        {
            Disparar();
            temporizador = 0f;
        }   
    }
    Vector3 ObtenerPosicionDisparo()
    {
        if (puntoDisparo != null) return puntoDisparo.position;

        // Centro del collider si existe, si no, del sprite
        if (col != null) return col.bounds.center;
        if (sr  != null) return sr.bounds.center;

        return transform.position;
    }

    void Disparar()
    {
        var pool = PoolBolas.Instance;
        if (pool == null) { Debug.LogError("Falta PoolBolas en escena."); return; }

        GameObject bola = pool.ObtenerBola();
        bola.transform.position = ObtenerPosicionDisparo();
        bola.transform.rotation = Quaternion.identity;

        var script = bola.GetComponent<BolaPinchos>();
        if (script == null) { Debug.LogError("El prefab no tiene BolaPinchos.cs"); return; }

        script.Disparar(direccionDisparo == Vector2.zero ? Vector2.left : direccionDisparo);
    }   
}
