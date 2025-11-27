using UnityEngine;

public class MenuPenguin : MonoBehaviour
{
    public float speed = 2f;
    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        anim.Play("Player Walk");  
    }

    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
        
    }
}