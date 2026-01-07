using TMPro;
using UnityEngine;

// Script de los coins
public class Reward : MonoBehaviour
{
    private TextMeshProUGUI scoreText;

    void Start()
    {
        GameObject scoreObject = GameObject.FindGameObjectWithTag("Score");
        if (scoreObject != null)
        {
            scoreText = scoreObject.GetComponent<TextMeshProUGUI>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            AudioManager.Instance.PlayCoinSound();
            GameManager.score += 1;
            scoreText.text = "  " + GameManager.score + " / ?"; 
            Destroy(gameObject); 
        }
    }
}