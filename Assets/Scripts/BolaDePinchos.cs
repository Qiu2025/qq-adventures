using System.Collections;
using UnityEngine;

public class BolaPinchos : MonoBehaviour
{
    public float velocidad = 5f;
    private Vector2 direccion;

    public void Disparar(Vector2 dir)
    {
        direccion = dir.normalized;
        gameObject.SetActive(true);
        StartCoroutine(DisappearInSeconds(3));
    }

    void Update()
    {
        transform.Translate(direccion * velocidad * Time.deltaTime);
    }

    private IEnumerator DisappearInSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        gameObject.SetActive(false);
    }
    
    private void OnBecameInvisible()
    {
        // Cuando sale de la cámara, se desactiva para volver al pool
        gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            gameObject.SetActive(false);
        }


        gameObject.SetActive(false);
    }
}
