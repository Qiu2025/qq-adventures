using UnityEngine;

public class PlataformaHorizontal : MonoBehaviour
{
    public float velocidad = 2f;
    public float distanciaMaxima = 2f;

    private float x0;
    private int dir = 1;

    void Start()
    {
        x0 = transform.position.x;
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
}
