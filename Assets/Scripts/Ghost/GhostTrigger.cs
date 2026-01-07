using UnityEngine;

// Script para triggers que activen GhostRuns
public class GhostTrigger : MonoBehaviour
{
    public float playCooldown = 1.5f;
    [SerializeField] private string runName;
    Animator animator;
    bool canPlay = true;
    float lastPlayedTime;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if(Time.time - lastPlayedTime > playCooldown)
        {
            canPlay = true;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && canPlay)
        {
            if(runName.Length == 0)
            {
                Debug.Log("GhostTrigger: runName vacio");
            }
            GhostRunner.Instance.PlayRunByName(runName);
            animator.SetTrigger("isPressed");
            canPlay = false;
            lastPlayedTime = Time.time;
        }
    }
}
