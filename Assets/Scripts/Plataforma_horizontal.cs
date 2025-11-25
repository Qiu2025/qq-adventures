using UnityEngine;

public class PlataformaHorizontal : MonoBehaviour
{
    public float velocidad = 2f;
    public float distanciaMaxima = 2f;

    private float x0;
    private int dir;

    [SerializeField] bool izqDch;

    [SerializeField] private GameObject prefabPuntoTrayecto; 
    [SerializeField] private int numeroPuntos = 16;             


    void Start()
    {
        if (izqDch) dir = -1;
        else dir = 1;

        x0 = transform.position.x;

        CrearPuntosTrayecto();
    }

    void FixedUpdate()
    {
        float nuevaX = transform.position.x + velocidad * Time.fixedDeltaTime * dir;

        if (Mathf.Abs(nuevaX - x0) >= distanciaMaxima)
            dir *= -1;

        transform.position += Vector3.right * velocidad * Time.fixedDeltaTime * dir;
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

       
        float inicio = x0 - distanciaMaxima;
        float fin = x0 + distanciaMaxima;

        Transform parent = transform.parent;

        for (int i = 0; i < numeroPuntos; i++)
        {
            float t = (i + 1f) / (numeroPuntos + 1f);
            float x = Mathf.Lerp(inicio, fin, t);

            Vector3 pos = new Vector3(x,transform.position.y,transform.position.z);

            Instantiate(prefabPuntoTrayecto, pos, Quaternion.identity, parent);
        }
    }
}
