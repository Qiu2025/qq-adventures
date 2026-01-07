using UnityEngine;

// Script para ponerlo sobre objetos que puedan matar al jugador
public class DeathZone : MonoBehaviour
{
    [SerializeField] private GameObject prefabExplosionPlumas;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {   
            Instantiate(prefabExplosionPlumas, collision.transform.position, Quaternion.identity);
            AudioManager.Instance.PlayDieSound();
            GameManager.Instance.StartCoroutine(GameManager.Instance.RespawnPlayerWithTransition());        
        }
    }
}
