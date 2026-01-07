using UnityEngine;
using System.Collections; 

// Script para mostrar el titulo "NIVEL 1: DESIERTO"
public class EfectoTitulo : MonoBehaviour
{
    public CanvasGroup canvasGroup; 
    public float tiempoAparicion = 1.5f;
    public float tiempoEspera = 3.0f;
    public float tiempoDesaparicion = 1.5f;

    void Start()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0;
            StartCoroutine(SecuenciaTitulo());
        }
        else
        {
            Debug.LogError("Falta asignar el CanvasGroup en el inspector");
        }
    }

    IEnumerator SecuenciaTitulo()
    {
        float timer = 0f;
        while (timer < tiempoAparicion)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / tiempoAparicion);
            yield return null;
        }
        canvasGroup.alpha = 1f;

      
        yield return new WaitForSeconds(tiempoEspera);

  
        timer = 0f;
        while (timer < tiempoDesaparicion)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / tiempoDesaparicion);
            yield return null;
        }
        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }
}