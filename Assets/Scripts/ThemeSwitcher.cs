using UnityEngine;
using System.Collections;

public class ThemeSwitcher : MonoBehaviour
{
    [Header("Configuración de Grupos")]
    public GameObject Spring;

    public GameObject Bosque;

    [Header("Tiempos")]
    public float tiempoEspera = 5f;    // Tiempo antes de empezar el cambio
    public float duracionTransicion = 2f; // Cuánto tarda el fundido (fade)

    void Start()
    {
        // 1. Estado inicial: Primavera visible, Desierto apagado
        Spring.SetActive(true);
        Bosque.SetActive(false);

        // Aseguramos que Primavera esté opaca (Alpha 1)
        CambiarAlphaGrupo(Spring, 1f);

        // 2. Iniciar la secuencia
        StartCoroutine(HacerTransicionSuave());
    }

    IEnumerator HacerTransicionSuave()
    {
        // Esperamos el tiempo de juego antes de cambiar
        yield return new WaitForSeconds(tiempoEspera);

        Debug.Log("Iniciando transición suave...");

        // 3. PREPARACIÓN:
        // Activamos el Desierto pero lo hacemos totalmente transparente (invisible)
        Bosque.SetActive(true); 
        CambiarAlphaGrupo(Bosque, 0f);

        float timer = 0f;

        // 4. BUCLE DE TRANSICIÓN (FADE):
        while (timer < duracionTransicion)
        {
            timer += Time.deltaTime;
            float avance = timer / duracionTransicion; // Va de 0 a 1

            // Primavera se desvanece (1 -> 0)
            CambiarAlphaGrupo(Spring, 1f - avance);
            
            // Desierto aparece (0 -> 1)
            CambiarAlphaGrupo(Bosque, avance);

            yield return null; // Esperar al siguiente frame
        }

        // 5. FINALIZACIÓN:
        // Aseguramos valores finales exactos y apagamos Primavera para ahorrar recursos
        CambiarAlphaGrupo(Bosque, 1f);
        Spring.SetActive(false);
    }

    // Función auxiliar que busca todos los sprites dentro del grupo y les cambia la transparencia
    void CambiarAlphaGrupo(GameObject grupo, float alpha)
    {
        // Busca todos los SpriteRenderer en el objeto y sus hijos
        SpriteRenderer[] sprites = grupo.GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer sprite in sprites)
        {
            Color colorActual = sprite.color;
            colorActual.a = alpha; // Modificamos solo el canal Alpha
            sprite.color = colorActual;
        }
    }
}