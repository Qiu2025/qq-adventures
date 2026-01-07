using UnityEngine;

// Script para mover la camara a una velocidad constante
// Usado por la camara del menu
public class CameraMove : MonoBehaviour
{
    public float speed = 0.2f;

    void Update()
    {
        transform.position += Vector3.right * speed * Time.deltaTime;
    }
}