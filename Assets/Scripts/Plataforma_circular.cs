using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Plataforma_circular : MonoBehaviour
{
    [SerializeField] private float radio = 2f;
    [SerializeField] private float velocidad = 2f;
    [SerializeField] private bool antiHorario;
    private float posInicialX;
    private float posInicialY;

    void Start()
    {
        posInicialX = gameObject.transform.position.x;
        posInicialY = gameObject.transform.position.y;
    }
    
    private void Update()
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
}
