using UnityEngine;

// Script para el prefab "Door"
public class Door : MonoBehaviour
{
    public Sprite keySprite;

    private bool canShowChat = true;   // evita spam del mensaje
    
    
    private float moveUpDistance = 5f;
    private float moveUpTime = 0.1f;
    private bool isOpening = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!canShowChat) return;  // evita parpadeo
        if (isOpening) return;

        if (collision.collider.CompareTag("Player"))
        {
            Key key = GameObject.FindGameObjectWithTag("PlayerContainer")
                .GetComponentInChildren<Key>();

            if (key != null)
            {
                Debug.Log("Door opened!");
                Destroy(key.gameObject);
                isOpening = true;
                // GetComponent<Collider2D>().enabled = false;  // optional but recommended
                StartCoroutine(OpenAndDisappear());
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
    
    // Método para animar a la puerta
    private System.Collections.IEnumerator OpenAndDisappear()
    {
        Vector3 start = transform.position;
        Vector3 end = start + Vector3.up * moveUpDistance;

        float t = 0f;
        while (t < moveUpTime)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / moveUpTime);
            transform.position = Vector3.Lerp(start, end, p);
            yield return null;
        }

        gameObject.SetActive(false); // or Destroy(gameObject);
    }

}