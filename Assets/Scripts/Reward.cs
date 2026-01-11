using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Reward : MonoBehaviour
{
    private TextMeshProUGUI scoreText;
    private RectTransform scoreRect;
    private Canvas scoreCanvas;
    private RectTransform canvasRect;

    private SpriteRenderer coinSR;
    private bool collected = false;

    [SerializeField] private float flyTime = 0.5f;
    [SerializeField] private float uiScale = 2f;
    

    // Offset en UI (en unidades del Canvas): X derecha/izquierda, Y arriba/abajo
    [SerializeField] private Vector2 targetUIOffset = new Vector2(-200f, -10f);

    void Start()
    {
        // Cachear sprite del coin y referencias del UI del score
        coinSR = GetComponent<SpriteRenderer>();

        GameObject scoreObject = GameObject.FindGameObjectWithTag("Score");
        if (scoreObject != null)
        {
            scoreText = scoreObject.GetComponent<TextMeshProUGUI>();
            if (scoreText != null)
            {
                scoreRect = scoreText.rectTransform;
                scoreCanvas = scoreText.canvas;
                canvasRect = scoreCanvas.GetComponent<RectTransform>();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Solo recoger una vez y solo si es el Player
        if (!collision.CompareTag("Player") || collected) return;
        collected = true;

        // Actualizar score y feedback
        AudioManager.Instance.PlayCoinSound();
        GameManager.score += 1;
        if (scoreText) scoreText.text = "  " + GameManager.score + " / ?";

        // Desactivar coin en el mundo (física y sprite)
        var col = GetComponent<Collider2D>();
        if (col) col.enabled = false;

        var rb = GetComponent<Rigidbody2D>();
        if (rb) rb.simulated = false;

        if (coinSR) coinSR.enabled = false;

        StartCoroutine(FlyCoinInUIAndDestroy());
    }

    private IEnumerator FlyCoinInUIAndDestroy()
    {
        // Si no hay UI/cámara, destruir sin animación
        if (scoreRect == null || scoreCanvas == null || canvasRect == null || Camera.main == null)
        {
            Destroy(gameObject);
            yield break;
        }

        Camera worldCam = Camera.main;
        Camera uiCam = GetUICamera(scoreCanvas);

        // Convertir posición del coin (mundo) a coordenadas locales del Canvas
        Vector2 startScreen = worldCam.WorldToScreenPoint(transform.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, startScreen, uiCam, out Vector2 startLocal
        );

        // Convertir posición del score (UI) a coordenadas locales del Canvas + offset
        Vector2 targetScreen = RectTransformUtility.WorldToScreenPoint(uiCam, scoreRect.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, targetScreen, uiCam, out Vector2 targetLocal
        );
        targetLocal += targetUIOffset;

        // Crear una imagen UI temporal usando el sprite del coin
        GameObject uiCoin = new GameObject("UI_FlyingCoin", typeof(RectTransform), typeof(Image));
        uiCoin.transform.SetParent(scoreCanvas.transform, false);

        RectTransform rt = uiCoin.GetComponent<RectTransform>();
        rt.anchoredPosition = startLocal;

        Image img = uiCoin.GetComponent<Image>();
        if (coinSR && coinSR.sprite) img.sprite = coinSR.sprite;
        img.preserveAspect = true;

        if (img.sprite != null)
        {
            rt.sizeDelta = img.sprite.rect.size * uiScale;
        }

        // Animar en espacio UI hasta el target y destruir
        float t = 0f;
        while (t < flyTime)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / flyTime);
            p = p * p * (3f - 2f * p);

            rt.anchoredPosition = Vector2.Lerp(startLocal, targetLocal, p);
            yield return null;
        }

        Destroy(uiCoin);
        Destroy(gameObject);
    }

    private Camera GetUICamera(Canvas c)
    {
        // Elegir cámara correcta según el Render Mode del Canvas
        if (c.renderMode == RenderMode.ScreenSpaceOverlay) return null;
        if (c.worldCamera != null) return c.worldCamera;
        return Camera.main;
    }
}
