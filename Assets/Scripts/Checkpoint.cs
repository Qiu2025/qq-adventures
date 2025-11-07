using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private Animator animator;
    private bool activated = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!activated && collision.CompareTag("Player"))
        {
            activated = true;
            animator.SetTrigger("Activate");
            GameManager.SetCheckpoint(transform.position);
            Debug.Log("Checkpoint activado!");
        }
    }
}