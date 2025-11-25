using UnityEngine;
using System.Collections;

public class DashPowerUp : MonoBehaviour
{
    private float respawnTime = 2f;

    private SpriteRenderer sr;
    private Collider2D col;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<CircleCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerMovement playerMovement = collision.gameObject.GetComponent<PlayerMovement>();

            // Permitir dash
            playerMovement.PowerUp();
            
            GameManager.PowerEffect();

            StartCoroutine(RespawnRoutine());
        }
    }

    private IEnumerator RespawnRoutine()
    {
        sr.enabled = false;
        col.enabled = false;

        // Reaparecer después de un rato
        yield return new WaitForSeconds(respawnTime);

        sr.enabled = true;
        col.enabled = true;
    }
}