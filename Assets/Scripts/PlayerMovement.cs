using System;
using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private float speed;
    [SerializeField] private float jumpingPower;
    private float horizontal;
    private bool grounded;
    public static bool isFacingLeft;

    [Header("Pistola de Agua")]
    [SerializeField] private Transform gun;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float resetRotationSpeed = 5f;
    [SerializeField] private ParticleSystem waterStream;

    private Quaternion gunInitialRotation;
    private SpriteRenderer gunSR;
    private Vector3 gunOriginalLocalPosition;
    private Vector3 firePointOriginalLocalPosition;
    private bool isAiming = false;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform groundCheck2;
    [SerializeField] private LayerMask groundLayer;

    [Header("Doble salto y caída lenta")]
    [SerializeField] private int maxJumps = 2;
    [SerializeField] private float normalGravity = 5f;
    [SerializeField] private float slowFallGravity = 0.5f;
    private int jumpsLeft;
    private bool wasGrounded;

    [Header("Temporizador")]
    [SerializeField] private Temporizador temporizador;

    [Header("Animations")]
    [SerializeField] Animator animator;

    [Header("Dash")]
    [SerializeField] private bool canDash = true;
    [SerializeField] private bool isDashing = false;
    [SerializeField] private float dashingPower;
    [SerializeField] private float dashingTime;
    [SerializeField] private float dashingCoolDown;
    [SerializeField] private TrailRenderer tr;

    void Start()
    {
        jumpsLeft = maxJumps;
        rb.gravityScale = normalGravity;
        wasGrounded = IsGrounded();
        isFacingLeft = true;

        gunSR = gun.GetComponent<SpriteRenderer>();
        gunInitialRotation = gun.localRotation;
        gunOriginalLocalPosition = gun.localPosition;
        firePointOriginalLocalPosition = firePoint.localPosition;
        UpdateGunPosition();
    }

    void Update()
    {
        if (isDashing) return;

        horizontal = Input.GetAxisRaw("Horizontal");
        grounded = IsGrounded();
        if (grounded) animator.SetBool("grounded", true); 
        else animator.SetBool("grounded", false);
        
        CheckJump();
        CheckGroundStatus();
        CheckDoubleJump();
        CheckDash();
        CheckWaterGun();
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

    private float nextEmitTime = 0f;
    private float emitInterval = 0.05f; // Intervalo entre emisiones (segundos)

    private void CheckWaterGun()
    {
        waterStream.transform.position = firePoint.position;
        
        if (Input.GetMouseButton(0))
        {
            if (!isAiming) 
            {
                isAiming = true;
            }
            
            RotateGunTowardsMouse();
            CheckGunFlip();

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 direction = mousePos - gun.position;
            float particleAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            waterStream.transform.rotation = Quaternion.Euler(0, 0, particleAngle);

            if (!waterStream.isPlaying)
            {
                waterStream.Play();
            }
            
            // Control por TIEMPO en lugar de por FRAME
            if (Time.time >= nextEmitTime)
            {
                waterStream.Emit(2); // 2 partículas cada 0.05 segundos
                nextEmitTime = Time.time + emitInterval;
            }
        }
        else
        {
            if (isAiming)
            {
                isAiming = false;
            }

            waterStream.Stop(true, ParticleSystemStopBehavior.StopEmitting);

            gun.localRotation = Quaternion.Lerp(
                gun.localRotation,
                gunInitialRotation,
                Time.deltaTime * resetRotationSpeed
            );
            firePoint.localRotation = Quaternion.Lerp(
                firePoint.localRotation,
                gunInitialRotation,
                Time.deltaTime * resetRotationSpeed
            );
            
            if (!waterStream.isEmitting)
            {
                float defaultAngle = isFacingLeft ? 180f : 0f;
                waterStream.transform.rotation = Quaternion.Lerp(
                    waterStream.transform.rotation,
                    Quaternion.Euler(0, 0, defaultAngle),
                    Time.deltaTime * resetRotationSpeed
                );
            }
        }
    }

    private void RotateGunTowardsMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePos - gun.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        if (isFacingLeft)
        {
            angle += 180f;
            gunSR.flipY = true;
        }
        else
        {
            gunSR.flipY = false;
        }
        
        gun.rotation = Quaternion.Euler(0, 0, angle);
        firePoint.rotation = Quaternion.Euler(0, 0, angle);
        
        // Para el Particle System, usa el ángulo SIN la corrección de 180°
        float particleAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        waterStream.transform.rotation = Quaternion.Euler(0, 0, particleAngle);
    }

    private void CheckGunFlip()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        bool aimingBackwards = (mousePos.x < transform.position.x && !isFacingLeft) ||
                        (mousePos.x > transform.position.x && isFacingLeft);
        
        if (aimingBackwards)
        {
            isFacingLeft = !isFacingLeft;
            sr.flipX = !sr.flipX;
            UpdateGunPosition();
            
            // Para el Particle System, calcular la dirección correcta sin la corrección de 180°
            Vector3 direction = mousePos - gun.position;
            float particleAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            waterStream.transform.rotation = Quaternion.Euler(0, 0, particleAngle);
        }
    }

    private void UpdateGunPosition()
    {
        if (isFacingLeft)
        {
            gunSR.flipX = true;
            gun.localPosition = new Vector3(-Mathf.Abs(gunOriginalLocalPosition.x), gunOriginalLocalPosition.y, gunOriginalLocalPosition.z);
            firePoint.localPosition = new Vector3(-Mathf.Abs(firePointOriginalLocalPosition.x), firePointOriginalLocalPosition.y, firePointOriginalLocalPosition.z);
        }
        else
        {
            gunSR.flipX = false;
            gun.localPosition = new Vector3(Mathf.Abs(gunOriginalLocalPosition.x), gunOriginalLocalPosition.y, gunOriginalLocalPosition.z);
            firePoint.localPosition = new Vector3(Mathf.Abs(firePointOriginalLocalPosition.x), firePointOriginalLocalPosition.y, firePointOriginalLocalPosition.z);
        }
    }

    private void CheckFlip()
    {
        if (isAiming) return;

        if ((isFacingLeft && horizontal > 0) || (!isFacingLeft && horizontal < 0))
        {
            isFacingLeft = !isFacingLeft;
            sr.flipX = !sr.flipX;
            UpdateGunPosition();
            
            // Cuando no estamos apuntando, el Particle System debe apuntar en la dirección por defecto
            if (!isAiming)
            {
                // Dirección por defecto: derecha si mira a la derecha, izquierda si mira a la izquierda
                float defaultAngle = isFacingLeft ? 180f : 0f;
                waterStream.transform.rotation = Quaternion.Euler(0, 0, defaultAngle);
            }
        }
    }

    private void CheckJump()
    {
        if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && jumpsLeft > 0)
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
        if (grounded && !wasGrounded)
        {
            animator.SetBool("isFalling", false);
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
        Quaternion originalRotation = transform.rotation;
        transform.rotation = Quaternion.Euler(0, 0, isFacingLeft ? 30f : -30f);
        rb.linearVelocity = new Vector2((isFacingLeft? -1:1) * dashingPower, 0f);
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
            temporizador.AumentarTiempo(5f);

            var otherSR = other.GetComponent<SpriteRenderer>();
            otherSR.enabled = false;

            var col = other.GetComponent<Collider2D>();
            col.enabled = false;
        }
        
    }





}