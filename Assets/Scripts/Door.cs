using UnityEngine;

// Script para el prefab "Door"
public class Door : MonoBehaviour
{
    public Sprite keySprite;

    private bool canShowChat = true;   // evita spam del mensaje
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!canShowChat) return;  // evita parpadeo

        if (collision.collider.CompareTag("Player"))
        {
            Key key = GameObject.FindGameObjectWithTag("PlayerContainer")
                .GetComponentInChildren<Key>();

            if (key != null)
            {
                Debug.Log("Door opened!");
                Destroy(key.gameObject);
                gameObject.SetActive(false);
                AudioManager.Instance.PlayDoorSound();
            }
            else
            {
                canShowChat = false; // bloquear nuevas llamadas
                GameManager.ShowChat(keySprite,0.2f,2f,0.2f, 5f, 0.1f, 0.25f );
                StartCoroutine(UnlockChat());  // reactivar después
                Debug.Log("Door is locked!");
            }
        }
    }

    private System.Collections.IEnumerator UnlockChat()
    {
        yield return new WaitForSeconds(2.5f);
        canShowChat = true;
    }
}