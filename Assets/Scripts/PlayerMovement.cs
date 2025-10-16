using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D.IK;

public class PlayerMovement : MonoBehaviour
{
    // --------------------------------------------- //

    [Header("Movimiento")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private float speed;
    [SerializeField] private float jumpingPower;
    private float horizontal;   // Movimiento horizontal del frame actual
    private bool grounded;  // GroundCheck del frame actual
    public static bool isFacingLeft;    // Controla direccion del sprite
    
    // --------------------------------------------- //

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform groundCheck2;
    [SerializeField] private LayerMask groundLayer;

    // --------------------------------------------- //

    [Header("Doble salto y caída lenta")]
    [SerializeField] private int maxJumps = 2;
    [SerializeField] private float normalGravity = 5f;
    [SerializeField] private float slowFallGravity = 0.5f;
    private int jumpsLeft;
    private bool wasGrounded; // Detecta si estaba en el aire

    // --------------------------------------------- //

    [Header("Temporizador")]
    [SerializeField] private Temporizador temporizador;

    // --------------------------------------------- //

    [Header("Animations")]
    [SerializeField] Animator animator;

    // --------------------------------------------- //
    [Header("Dashing")]
    [SerializeField] private bool canDash = true;
    [SerializeField] private bool isDashing = false;
    [SerializeField] private float dashingPower = 25;
    [SerializeField] private float dashingTime = 0.1f;
    [SerializeField] private float dashingCoolDown = 1f;
    [SerializeField] private TrailRenderer tr;



    // --------------------------------------------- //

    void Start()
    {
        jumpsLeft = maxJumps;
        rb.gravityScale = normalGravity;
        wasGrounded = IsGrounded();
        isFacingLeft = true;
    }

    void Update()
    {
        if (isDashing) return;

        horizontal = Input.GetAxisRaw("Horizontal");
        grounded = IsGrounded();
        if (grounded) animator.SetBool("grounded", true); else animator.SetBool("grounded", false);
        CheckJump();
        CheckGroundStatus();
        CheckDoubleJump();
        CheckDash();
    }

    void FixedUpdate()
    {
        if (isDashing) return;

        rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocity.y);
        CheckFlip();
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
        if (Input.GetKeyDown(KeyCode.W) && jumpsLeft > 0)
        {
            if (!grounded) jumpsLeft = Math.Min(jumpsLeft, 1);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
            jumpsLeft--;
            animator.SetBool("isFalling", false);
            animator.SetBool("isJumping", true);
        }
    }

    private void CheckGroundStatus()
    {
        // Reinicia saltos solo cuando acaba de tocar el suelo
        if (grounded && !wasGrounded)
        {
            jumpsLeft = maxJumps;
        }

        wasGrounded = grounded;
    }
    
    private void CheckDoubleJump()
    {
        if (!grounded && rb.linearVelocity.y < 0f)
        {
            animator.SetBool("isFalling", true);
            animator.SetBool("isJumping", false);
            
            if (Input.GetKey(KeyCode.Space))
            {
                jumpsLeft = 0;
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
        if ((isFacingLeft && horizontal > 0) || (!isFacingLeft && horizontal < 0))
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

    private void CheckDash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
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
        rb.linearVelocity = new Vector2((isFacingLeft? -1:1) * dashingPower, 0f);
        tr.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;
        rb.gravityScale = originalGravity;
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