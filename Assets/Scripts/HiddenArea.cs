using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;

// Script para hacer fade-in / fade-out de las zonas ocultas
public class HiddenZone : MonoBehaviour
{
    private TilemapRenderer tr;
    private float fadeDuration = 0.1f; // seconds
    private Coroutine fadeCoroutine;

    void Start()
    {
        tr = GetComponent<TilemapRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeTo(0f)); // fade out
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeTo(1f)); // fade in
        }
    }

    private IEnumerator FadeTo(float targetAlpha)
    {
        Color startColor = tr.material.color;
        float startAlpha = startColor.a;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
            Color newColor = new Color(startColor.r, startColor.g, startColor.b, newAlpha);
            tr.material.color = newColor;
            yield return null;
        }

        tr.material.color = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);
    }
}