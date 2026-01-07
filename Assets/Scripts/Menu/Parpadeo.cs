using UnityEngine;
using UnityEngine.UI; 

// Script para realizar el efecto parpadeo del texto "PRESS ANY KEY" del menu
public class EfectoParpadeo : MonoBehaviour
{
    [Header("Configuración")]
    public float velocidad = 5f;     
    public float alphaMinimo = 0f;   // 0 = invisible, 1 = visible
    public float alphaMaximo = 1f;   

    private Image imagen;

    void Start()
    {
        imagen = GetComponent<Image>();
    }

    void Update()
    {
        if (imagen != null)
        {
            float alpha = (Mathf.Sin(Time.time * velocidad) + 1.0f) / 2.0f;

            alpha = Mathf.Lerp(alphaMinimo, alphaMaximo, alpha);

            Color colorActual = imagen.color;
            colorActual.a = alpha;
            imagen.color = colorActual;
        }
    }
}