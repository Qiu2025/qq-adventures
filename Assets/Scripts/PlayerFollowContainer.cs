using UnityEngine;

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
