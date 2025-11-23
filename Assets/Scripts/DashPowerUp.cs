using UnityEditor.AssetImporters;
using UnityEngine;

public class DashPowerUp : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerMovement playerMovement =  collision.gameObject.GetComponent<PlayerMovement>();
            playerMovement.canDash =  true;
            playerMovement.dashedInAir = false;
            Destroy(gameObject);
        }
    }
}
