using UnityEngine;
using System.Collections;

public class ThemeSwitcher : MonoBehaviour
{
    [Header("Configuración de Grupos")]
    public GameObject Spring;

    public GameObject Bosque;

    [Header("Tiempos")]
    public float tiempoEspera = 5f;    // Tiempo antes de empezar el cambio
    public float duracionTransicion = 2f; // Duracion del fade

    void Start()
    {
        Spring.SetActive(true);
        Bosque.SetActive(false);

        // Aseguramos que Primavera esté opaca (Alpha 1)
        CambiarAlphaGrupo(Spring, 1f);

        // Iniciar la secuencia
        StartCoroutine(HacerTransicionSuave());
    }

    IEnumerator HacerTransicionSuave()
    {
        yield return new WaitForSeconds(tiempoEspera);

        // Activamos el Desierto pero lo hacemos totalmente transparente
        Bosque.SetActive(true); 
        CambiarAlphaGrupo(Bosque, 0f);

        float timer = 0f;

        // Fade
        while (timer < duracionTransicion)
        {
            timer += Time.deltaTime;
            float avance = timer / duracionTransicion;

            // Primavera 1 -> 0
            CambiarAlphaGrupo(Spring, 1f - avance);
            
            // Desierto 0 -> 1
            CambiarAlphaGrupo(Bosque, avance);

            yield return null;
        }

        CambiarAlphaGrupo(Bosque, 1f);
        Spring.SetActive(false);
    }

    // Función auxiliar que busca todos los sprites dentro del grupo y les cambia la transparencia
    void CambiarAlphaGrupo(GameObject grupo, float alpha)
    {
        SpriteRenderer[] sprites = grupo.GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer sprite in sprites)
        {
            Color colorActual = sprite.color;
            colorActual.a = alpha;
            sprite.color = colorActual;
        }
    }
}