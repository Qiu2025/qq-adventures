using UnityEngine;

public class DeathZone : MonoBehaviour
{
    [SerializeField] private GameObject prefabExplosionPlumas;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {   
            Instantiate(prefabExplosionPlumas, transform.position, Quaternion.identity);
            AudioManager.Instance.PlayDieSound();
            GameManager.Instance.StartCoroutine(GameManager.Instance.RespawnPlayerWithTransition());        
        }
    }
}
