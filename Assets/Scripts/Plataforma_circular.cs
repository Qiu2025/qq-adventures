using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Plataforma_circular : MonoBehaviour
{
    public float radio = 2f;
    public float velocidad = 2f;
    public Transform puntoVertice;
    public bool cambiarOrientacion = false;

    private void Update()
    {
        MoverPlataforma();
    }

    void MoverPlataforma()
    {
        float sentido = cambiarOrientacion ? 1f : -1f;
        float angulo = Time.time * velocidad * sentido;
        float x = puntoVertice.position.x + Mathf.Cos(angulo) * radio;
        float y = puntoVertice.position.y + Mathf.Sin(angulo) * radio;
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
}
