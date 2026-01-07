using UnityEngine;

// Metido en un empty, usado para mostrar los "bocadillos" y el efecto rayo de powerup junto al jugador
public class PlayerFollowContainer : MonoBehaviour
{
    private GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        gameObject.transform.position = player.transform.position;
    }
}
