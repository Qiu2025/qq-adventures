using UnityEngine;

// Script de las plataformas de movimiento circular
public class Plataforma_circular : MonoBehaviour
{
    [SerializeField] private float radio = 2f;
    [SerializeField] private float velocidad = 2f;
    [SerializeField] private bool antiHorario;
    private float posInicialX;
    private float posInicialY;

    [SerializeField] private GameObject prefabPuntoTrayecto; 
    [SerializeField] private int numeroPuntos = 24;           

    void Start()
    {
        posInicialX = gameObject.transform.position.x;
        posInicialY = gameObject.transform.position.y;

        CrearPuntosTrayecto();
    }
    
    private void FixedUpdate()
    {
        MoverPlataforma();
    }

    void MoverPlataforma()
    {
        float sentido = antiHorario ? 1f : -1f;
        float angulo = Time.time * velocidad * sentido;
        float x = posInicialX + Mathf.Cos(angulo) * radio;
        float y = posInicialY + Mathf.Sin(angulo) * radio;
        transform.position = new Vector3(x, y, transform.position.z);
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

        Transform parent = transform.parent;

        for (int i = 0; i < numeroPuntos; i++)
        {
            float t = (float)i / numeroPuntos;
            float angulo = t * Mathf.PI * 2f;

            float x = posInicialX + Mathf.Cos(angulo) * radio;
            float y = posInicialY + Mathf.Sin(angulo) * radio;

            Vector3 pos = new Vector3(x, y, transform.position.z);
            Instantiate(prefabPuntoTrayecto, pos, Quaternion.identity, parent);
        }
    }

}
