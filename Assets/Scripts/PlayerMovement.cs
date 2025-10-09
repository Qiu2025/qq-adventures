using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // Para movimiento del jugador
    [SerializeField] private Rigidbody2D rb;
    private float horizontal;
    private float speed = 8f;
    private float jumpingPower = 16f;


    // Para groundcheck
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform groundCheck2;
    [SerializeField] private LayerMask groundLayer;


    // Para hacer flip de la imagen
    [SerializeField] private SpriteRenderer sr;
    private bool isFacingLeft = true;

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            rb.linearVelocityY = jumpingPower;
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocityY);

        CheckFlip();
    }

    private void CheckFlip()
    {
        if (isFacingLeft && horizontal > 0 || !isFacingLeft && horizontal < 0)
        {
            isFacingLeft = !isFacingLeft;
            sr.flipX = !sr.flipX;
        }
    }
    
    private bool IsGrounded()
    {
        return Physics2D.Raycast(groundCheck.position, Vector2.down, 0.1f, groundLayer) ||
                Physics2D.Raycast(groundCheck2.position, Vector2.down, 0.1f, groundLayer);
    }
}
