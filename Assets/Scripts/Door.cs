using UnityEngine;
using System.Collections;

// Script para el prefab "Door"
public class Door : MonoBehaviour
{
    public Sprite keySprite;

    private bool canShowChat = true;   // evita spam del mensaje
    
    private float moveUpDistance = 5f;
    private float moveUpTime = 0.1f;
    [HideInInspector] public bool abierta = false; // para que el trigger lo sepa

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (abierta) return; // si ya está abierta, no hace nada

        if (collision.collider.CompareTag("Player"))
        {
            Key key = GameObject.FindGameObjectWithTag("PlayerContainer")
                .GetComponentInChildren<Key>();

            if (key != null)
            {
                // SI TIENE LA LLAVE: Abrimos directamente sin mostrar el chat
                Debug.Log("Door opened!");
                Destroy(key.gameObject);
                abierta = true;
                StopAllCoroutines(); 
                StartCoroutine(OpenAndDisappear());
                AudioManager.Instance?.PlayDoorSound();
            }
            else
            {
                // SI NO TIENE LA LLAVE: Mostramos el chat 
                if (canShowChat)
                {
                    canShowChat = false; 
                    GameManager.ShowChat(keySprite, 0.2f, 2f, 0.2f, 5f, 0.1f, 0.25f);
                    StartCoroutine(UnlockChat()); 
                    Debug.Log("Door is locked!");
                }
            }
        }
    }

    private System.Collections.IEnumerator UnlockChat()
    {
        yield return new WaitForSeconds(2.5f);
        if (!abierta) canShowChat = true;
    }
    
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

        gameObject.SetActive(false); 
    }
}