using UnityEngine;

public class Door : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Key key = collision.collider.GetComponentInChildren<Key>();
            if (key != null)
            {
                Debug.Log("Door opened!");
                Destroy(key.gameObject);
                gameObject.SetActive(false);
            }
            else
            {
                Debug.Log("Door is locked!");
            }
        }
    }
}