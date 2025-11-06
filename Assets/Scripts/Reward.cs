using TMPro;
using UnityEngine;

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
            scoreText.text = "Secrets: "+ (GameManager.score += 1) + "/3";
            Destroy(gameObject); // Desaparece el reward
        }
    }
}
