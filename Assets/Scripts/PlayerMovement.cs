using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private float speed = 8f;
    [SerializeField] private float jumpingPower = 12f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform groundCheck2;
    [SerializeField] private LayerMask groundLayer;

    [Header("Doble salto y caída lenta")]
    [SerializeField] private int maxJumps = 2;
    [SerializeField] private float normalGravity = 3f;
    [SerializeField] private float slowFallGravity = 0.5f;

    [Header("Temporizador")]
    [SerializeField] private Temporizador temporizador;

    private float horizontal;
    private bool isFacingLeft = true;
    private int jumpsLeft;
    private bool wasGrounded; // Para detectar cuando acaba de tocar el suelo
    private bool isRolling;
    private float rollDuration = 0.3f; // Duración del roll en segundos
    private float rollTimer = 0f;

    void Start()
    {
        jumpsLeft = maxJumps;
        rb.gravityScale = normalGravity;
        wasGrounded = IsGrounded();
    }

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        bool grounded = IsGrounded();

        // Reinicia saltos solo cuando acaba de tocar el suelo
        if (grounded && !wasGrounded)
        {
            jumpsLeft = maxJumps;
        }
        wasGrounded = grounded;

        // --- Salto ---
        if (Input.GetKeyDown(KeyCode.Space) && jumpsLeft > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
            jumpsLeft--; // decrementa siempre al saltar
        }

        // --- Caída lenta mientras cae y mantiene espacio ---
        if (!grounded && rb.linearVelocity.y < 0f)
        {
            bool holdingSpace = Keyboard.current != null
                ? Keyboard.current.spaceKey.isPressed
                : Input.GetKey(KeyCode.Space);

            rb.gravityScale = holdingSpace ? slowFallGravity : normalGravity;
        }
        else
        {
            rb.gravityScale = normalGravity;
        }

        // --- Roll ---
        if (Input.GetKeyDown(KeyCode.S) && (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) && grounded && !isRolling)
        {
            isRolling = true;
            rollTimer = rollDuration; // Inicia el temporizador
            float rollDirection = Input.GetKey(KeyCode.A) ? -1f : 1f;
            rb.linearVelocity = new Vector2(rollDirection * speed, rb.linearVelocity.y);
            Debug.Log("Roll!");
            sr.flipY = true;
        }
        if (isRolling)
        {
            rollTimer -= Time.deltaTime;
            if (rollTimer <= 0f)
            {
                isRolling = false;
                sr.flipY = false;
                Debug.Log("End Roll");
            }
        }
    }

    void FixedUpdate()
    {
        if (!isRolling)
        {
            rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocity.y);
            CheckFlip();
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
    void OnTriggerEnter2D(Collider2D other)
    {
        // Opción A: por Tag (pon al helado el tag "IceCream")
        if (other.CompareTag("IceCream"))
        {
            if (temporizador != null)
            {
                temporizador.AumentarTiempo(5f);
            }

            // 🔹 1. Desactivar el sprite (ocultar el helado)
            var sr = other.GetComponent<SpriteRenderer>();
            if (sr) sr.enabled = false;

            // 🔹 2. Desactivar su collider para que no se pueda volver a tocar
            var col = other.GetComponent<Collider2D>();
            if (col) col.enabled = false;
        }
    }
    private bool IsGrounded()
    {
        return Physics2D.Raycast(groundCheck.position, Vector2.down, 0.1f, groundLayer) ||
               Physics2D.Raycast(groundCheck2.position, Vector2.down, 0.1f, groundLayer);
    }
}