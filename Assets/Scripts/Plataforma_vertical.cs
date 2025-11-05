using UnityEngine;

public class PlataformaVertical : MonoBehaviour
{
    public float velocidad = 2f;
    public float distanciaMaxima = 2f;

    private float y0;
    private int dir = 1;

    void Start()
    {
        y0 = transform.position.y;
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
}
