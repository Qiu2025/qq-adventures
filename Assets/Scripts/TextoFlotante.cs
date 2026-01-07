using UnityEngine;
using TMPro;

// Script para mostrar texto flotante
// Usado por el checkpoint para mostrar el tiempo de llegada
public class TextoFlotante : MonoBehaviour
{
    [SerializeField] private float velocidadSubida = 1f;
    [SerializeField] private float tiempoVida = 2f;
    private TextMeshProUGUI textoMesh;
    private Color colorInicial;

    void Awake()
    {
        textoMesh = GetComponent<TextMeshProUGUI>();
        colorInicial = textoMesh.color;
    }

    public void ConfigurarTexto(string mensaje)
    {
        textoMesh.text = mensaje;
        Destroy(transform.parent.gameObject, tiempoVida); 
    }

    void Update()
    {
        // Sube suavemente
        transform.parent.Translate(Vector3.up * velocidadSubida * Time.deltaTime);

        // Fade out
        float alpha = textoMesh.color.a - (Time.deltaTime / tiempoVida);
        textoMesh.color = new Color(colorInicial.r, colorInicial.g, colorInicial.b, alpha);
    }
}