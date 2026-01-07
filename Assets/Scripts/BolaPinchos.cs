using System.Collections;
using UnityEngine;

// Script del prefab bola que dispara el cactus
public class BolaPinchos : MonoBehaviour
{
    public float velocidad = 5f;
    private Vector2 direccion;

    public void Disparar(Vector2 dir,float tiempoVida)
    {
        direccion = dir.normalized;
        gameObject.SetActive(true);
        StartCoroutine(DisappearInSeconds(tiempoVida));
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
        gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            gameObject.SetActive(false);
        }


        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            gameObject.SetActive(false);
        }
    }
}

