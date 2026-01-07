using UnityEngine;

// Script que mueve todos las imagenes de fondo del menu para el efecto parallax
// Metido en cada una de las imagenes de fondo del menu
public class BackgroundController : MonoBehaviour
{
    private float startPos, length;
    public GameObject cam;
    public float parallaxEffect;

    void OnEnable() 
    {
        startPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void FixedUpdate()
    {
        float distance = cam.transform.position.x * parallaxEffect;
        float movement = cam.transform.position.x * (1 - parallaxEffect);

        transform.position = new Vector3(startPos + distance, transform.position.y, transform.position.z);

        if (movement > startPos + length) startPos += length;
        else if (movement < startPos - length) startPos -= length;
    }
}