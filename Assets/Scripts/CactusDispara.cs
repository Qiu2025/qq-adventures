using UnityEngine;

public class CactusDispara : MonoBehaviour
{
    [Header("Disparo")]
    public float tiempoEntreDisparos = 2f;
    public Vector2 direccionDisparo = Vector2.left;
    public float tiempoVidaProyectil = 3f;   
    public Transform puntoDisparo; // opcional. Si no se asigna, usa centro del sprite/collider.

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
        GameObject bola = PoolBolas.Instance != null ? PoolBolas.Instance.ObtenerBola() : null;

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

        script.Disparar(direccionDisparo == Vector2.zero ? Vector2.left : direccionDisparo, tiempoVidaProyectil);
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

        script.Disparar(direccionDisparo == Vector2.zero ? Vector2.left : direccionDisparo,tiempoVidaProyectil);
    }   
}
