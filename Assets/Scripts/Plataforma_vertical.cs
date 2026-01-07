using UnityEngine;

// Script de las plataformas de movimiento vertical
public class PlataformaVertical : MonoBehaviour
{
    public float velocidad = 2f;
    public float distanciaMaxima = 2f;

    private float y0;
    private int dir = 1;

    [SerializeField] private GameObject prefabPuntoTrayecto; 
    [SerializeField] private int numeroPuntos = 16;          

    void Start()
    {
        y0 = transform.position.y;
        CrearPuntosTrayecto();
    }

    void FixedUpdate()
    {
        float nuevaX = transform.position.y + velocidad * Time.fixedDeltaTime * dir;

        if (Mathf.Abs(nuevaX - y0) >= distanciaMaxima)
            dir *= -1;

        transform.position += Vector3.up * velocidad * Time.fixedDeltaTime * dir;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
            collision.transform.SetParent(transform);
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
            collision.transform.SetParent(null);
    }

    private void CrearPuntosTrayecto()
    {
        if (prefabPuntoTrayecto == null || numeroPuntos <= 0)
            return;

        float inicio = y0 - distanciaMaxima;
        float fin = y0 + distanciaMaxima;

        // Los cuelgo del mismo padre que la plataforma para que NO se muevan con ella
        Transform parent = transform.parent;

        for (int i = 0; i < numeroPuntos; i++)
        {
            float t = (i + 1f) / (numeroPuntos + 1f);
            float y = Mathf.Lerp(inicio, fin, t);

            Vector3 pos = new Vector3(transform.position.x, y, transform.position.z);
            Instantiate(prefabPuntoTrayecto, pos, Quaternion.identity, parent);
        }
    }
}
