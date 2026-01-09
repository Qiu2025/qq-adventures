using UnityEngine;

public class CreditsScript : MonoBehaviour
{
    public float scrollSpeed = 40f;

    public RectTransform rectTransform;
    private Vector2 startPosition;

    void Awake()
    {
        startPosition = rectTransform.anchoredPosition;
    }

    void Update()
    {
        rectTransform.anchoredPosition +=
            Vector2.up * scrollSpeed * Time.unscaledDeltaTime;
    }

    void OnDisable()
    {
        rectTransform.anchoredPosition = startPosition;
    }
}
