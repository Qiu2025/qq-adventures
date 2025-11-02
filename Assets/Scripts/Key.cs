using UnityEngine;

public class Key : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            transform.SetParent(col.transform);       // make child of player
            transform.localScale *= 0.5f;             // make smaller
            GetComponent<Collider2D>().enabled = false;
        }
    }
}
