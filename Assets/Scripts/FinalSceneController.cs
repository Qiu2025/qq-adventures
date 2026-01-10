using System.Collections;
using UnityEngine;
using TMPro;

public class FinalSceneController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private CanvasGroup fadeImage;
    [SerializeField] private CanvasGroup statsPanel;
    [SerializeField] private CanvasGroup creditsPanel;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI statsText;

    [Header("Timings")]
    [SerializeField] private float fadeTime = 1.2f;
    [SerializeField] private float charDelay = 0.03f;
    [SerializeField] private float timeBeforeCredits = 4f;

    private void Start()
    {
        Cursor.visible = true;
        StartCoroutine(Sequence());
    }

    private IEnumerator Sequence()
    {
        
        yield return FadeCanvas(fadeImage, 1f, 0f, fadeTime);
        yield return FadeCanvas(statsPanel, 0f, 1f, 0.8f);
        string fullText = BuildStatsText();
        yield return TypeText(fullText);
        
        yield return new WaitForSeconds(timeBeforeCredits);
        
        yield return FadeCanvas(fadeImage, 0f, 1f, fadeTime);
        
        statsPanel.alpha = 0f;
        
        creditsPanel.alpha = 1f;
        
    }


    // ---------------- TEXTOS ----------------

    private string BuildStatsText()
    {
        string text = "";

        float total = GameManager.GetTotalTime();
        text += $"Tiempo total: {FormatTime(total)}\n\n";

        foreach (var zone in GameManager.GetZoneTimes())
        {
            string cleanKey = CleanZoneKey(zone.Key);
            string zoneTitle = GetZoneTitle(cleanKey);
            text += $"{zoneTitle}: {FormatTime(zone.Value)}\n\n";
        }

        text += $"Monedas recogidas: {GameManager.score}";
        return text;
    }

    private string CleanZoneKey(string key)
    {
        int dotIndex = key.IndexOf('.');
        if (dotIndex != -1)
            key = key.Substring(dotIndex + 1);

        return key.Trim();
    }

    private string GetZoneTitle(string key)
    {
        switch (key)
        {
            case "Salto": return "Zona Salto";
            case "DobleSalto": return "Zona Doble Salto";
            case "Dash": return "Zona Dash";
            case "PowerUps": return "Zona Power Ups";
            default: return $"Zona {key}";
        }
    }

    // ---------------- EFECTOS ----------------

    private IEnumerator TypeText(string fullText)
    {
        statsText.text = "";

        foreach (char c in fullText)
        {
            statsText.text += c;
            yield return new WaitForSecondsRealtime(charDelay);
        }
    }

    private IEnumerator FadeCanvas(CanvasGroup cg, float from, float to, float time)
    {
        float t = 0f;
        cg.alpha = from;

        while (t < time)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(from, to, t / time);
            yield return null;
        }

        cg.alpha = to;
    }

    private string FormatTime(float t)
    {
        int min = Mathf.FloorToInt(t / 60f);
        int sec = Mathf.FloorToInt(t % 60f);
        int cent = Mathf.FloorToInt((t * 100f) % 100f);
        return $"{min:00}:{sec:00}.{cent:00}";
    }
}
