using UnityEngine;

public class DeathZone : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {   
            /*GAME OVER*/
            // GameManager.SetGameOver(true);
            
            /* RESPAWN */
            GameManager.RespawnPlayer();
            
            Debug.Log("You died!");
        }
    }
}
