using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // --------------------------------------------- //
    [Header("Selección de mecánicas")]
    [SerializeField] private bool allowDoubleJump;
    [SerializeField] private bool allowDash;
    [SerializeField] private bool allowGlide;


    // --------------------------------------------- //

    [Header("Movimiento")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private float speed;
    [SerializeField] private float jumpingPower;
    [SerializeField] private float secondJumpingPower;
    private float horizontal;   // Movimiento horizontal del frame actual
    private bool grounded;  // GroundCheck del frame actual
    public static bool isFacingRight;    // Controla direccion del sprite
    
    // --------------------------------------------- //

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform groundCheck2;
    [SerializeField] private LayerMask groundLayer;

    // --------------------------------------------- //

    [Header("Doble salto y caída lenta")]
    [SerializeField] private int maxJumps;
    [SerializeField] private float normalGravity;
    [SerializeField] private float slowFallGravity;
    [SerializeField] private float coyoteTime; // Tolerancia al considerar primer salto
    private int usedJumps;
    private float lastGroundedTime;
    private bool wasGrounded; // Detecta si estaba en el aire

    // --------------------------------------------- //

    [Header("Temporizador")]
    [SerializeField] private Temporizador temporizador;

    // --------------------------------------------- //

    [Header("Animations")]
    [SerializeField] Animator animator;

    // --------------------------------------------- //

    [Header("Dash")]
    [SerializeField] private bool canDash = true;
    [SerializeField] private bool isDashing = false;
    [SerializeField] private float dashingPower;
    [SerializeField] private float dashingTime;
    [SerializeField] private float dashingCoolDown;
    [SerializeField] private TrailRenderer tr;

    // --------------------------------------------- //

    void Start()
    {
        maxJumps = 2;
        usedJumps = 0;
        normalGravity = 5f;
        slowFallGravity = 0.5f;
        coyoteTime = 0.08f;
        rb.gravityScale = normalGravity;
        wasGrounded = IsGrounded();
        lastGroundedTime = wasGrounded ? Time.time : -999f;
        isFacingRight = true;
    }

    void Update()
    {
        if (isDashing) return;

        horizontal = Input.GetAxisRaw("Horizontal");
        
        
        grounded = IsGrounded();

        CheckGroundedAnimation();
        CheckJump();
        CheckGroundStatus();
        CheckGlide();
        CheckDash();
    }

    void FixedUpdate()
    {
        if (isDashing) return;

        rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocity.y);
        CheckFlip();
        CheckWalkingAnimation();
    }

    private void CheckGroundedAnimation()
    {
        if (grounded)
        {
            animator.SetBool("grounded", true);
        }
        else
        {
            animator.SetBool("grounded", false);
        }
    }

    private void CheckWalkingAnimation()
    {
        if (IsGrounded() && horizontal != 0)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }

    private void CheckJump()
    {        
        bool jumpPressed = Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.JoystickButton0);

        if (!jumpPressed) return;
    
        // Consideramos "en suelo" si actualmente grounded o si estamos dentro del coyote time
        bool currentlyGrounded = grounded || (Time.time - lastGroundedTime) <= coyoteTime;

        if (currentlyGrounded)
        {
            // Primer salto (o salto desde coyote time)
            rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpingPower);
            usedJumps = 1; // hemos usado el primer salto
            animator.SetBool("isFalling", false);
            animator.SetBool("isJumping", true);
            return;
        }

        // Si se ha seleccionado no permitir la mecánica de doble salto en la escena actual
        if (!allowDoubleJump) return;

        // Si no estamos en suelo, permitir el doble salto si queda (usedJumps < maxJumps)
        if (usedJumps < maxJumps)
        {
            // Segundo salto
            rb.linearVelocity = new Vector2(rb.linearVelocityX, secondJumpingPower);
            usedJumps++;
            animator.SetBool("isFalling", false);
            animator.SetBool("isJumping", true);
            return;
        }

        // Estar aqui = no quedan saltos disponibles
    }

    private void CheckGroundStatus()
    {
        // Reinicia saltos solo cuando acaba de tocar el suelo
        if (grounded && !wasGrounded)
        {
            animator.SetBool("isFalling", false);
            usedJumps = 0;
        }

        // Actualiza lastGroundedTime cuando estamos en suelo (para coyote time)
        if (grounded)
        {
            lastGroundedTime = Time.time;
        }

        wasGrounded = grounded;
    }
    
    private void CheckGlide()
    {
        if (!grounded && rb.linearVelocity.y < 0f)
        {
            animator.SetBool("isFalling", true);
            animator.SetBool("isJumping", false);

            // Si se ha seleccionado no permitir la mecánica de gliding en la escena actual
            if (!allowGlide) return;
            
            if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.JoystickButton0))
            {
                // No permitir saltos si mantiene presionado espacio (es decir, hace glide)
                usedJumps = 2;
                rb.gravityScale = slowFallGravity;
            }
            else
            {
                rb.gravityScale = normalGravity;
            }
        }
        else
        {
            rb.gravityScale = normalGravity;
        }
    }

    private void CheckFlip()
    {
        if ((isFacingRight && horizontal < 0) || (!isFacingRight && horizontal > 0))
        {
            isFacingRight = !isFacingRight;
            sr.flipX = !sr.flipX;
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.Raycast(groundCheck.position, Vector2.down, 0.1f, groundLayer) ||
               Physics2D.Raycast(groundCheck2.position, Vector2.down, 0.1f, groundLayer);
    }

    private void CheckDash()
    {
        // Si se ha seleccionado no permitir la mecánica de dash en la escena actual
        if (!allowDash) return;
        

        if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKey(KeyCode.JoystickButton7)) && canDash)
        {
            StartCoroutine(Dash());
        }
    }
    
    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        Quaternion originalRotation = transform.rotation;
        transform.rotation = Quaternion.Euler(0, 0, isFacingRight ? -30f : 30f);
        rb.linearVelocity = new Vector2((isFacingRight? 1:-1) * dashingPower, 0f);
        tr.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;
        rb.gravityScale = originalGravity;
        transform.rotation = originalRotation;
        isDashing = false;
        yield return new WaitForSeconds(dashingCoolDown);
        canDash = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("IceCream"))
        {
            if (temporizador != null)
            {
                temporizador.AumentarTiempo(5f);
            }

            // Desactivar el sprite (ocultar el helado)
            var sr = other.GetComponent<SpriteRenderer>();
            if (sr) sr.enabled = false;

            // Desactivar su collider para que no se pueda volver a tocar
            var col = other.GetComponent<Collider2D>();
            if (col) col.enabled = false;
        }
    }
}