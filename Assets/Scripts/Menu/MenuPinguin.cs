using UnityEngine;

// Script para mover el pinguino a una velocidad constante
public class MenuPenguin : MonoBehaviour
{
    public float speed = 2f;
    
    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }
}