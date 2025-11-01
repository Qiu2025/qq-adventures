using UnityEngine;

public class WaterGun : MonoBehaviour
{
    [SerializeField] private PlayerMovement player;       
    [SerializeField] private Transform gun;              // Transform del sprite de la pistola
    [SerializeField] private float resetRotationSpeed = 5f; 

    private Quaternion initialRotation; 
    private SpriteRenderer sr;
    private bool wasFacingLeft;
    private Vector3 originalLocalPosition;
    private bool isAiming = false;
    private bool forcedFlip = false; // Para evitar conflicto con el flip automático del player

    void Start()
    {
        sr = gun.GetComponent<SpriteRenderer>();
        initialRotation = gun.localRotation;
        wasFacingLeft = PlayerMovement.isFacingLeft;
        originalLocalPosition = gun.localPosition;
        
        UpdateGunPosition();
    }

    void Update()
    {
        // Solo permitir flip automático cuando no estamos apuntando
        if (!isAiming)
        {
            FlipAndAdjustPosition();
        }

        // Mientras mantienes clic izquierdo → apuntar al ratón
        if (Input.GetMouseButton(0))
        {
            isAiming = true;
            RotateTowardsMouse();
            CheckAimingDirection();
        }
        else
        {
            isAiming = false;
            // Si había un flip forzado por apuntar, restaurar la dirección original
            if (forcedFlip)
            {
                RestoreOriginalDirection();
            }
            
            // Volver suavemente a la rotación inicial
            gun.localRotation = Quaternion.Lerp(
                gun.localRotation,
                initialRotation,
                Time.deltaTime * resetRotationSpeed
            );
        }
    }

    private void RotateTowardsMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePos - gun.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        // Si el sprite está flipado, ajustamos el ángulo
        if (sr.flipX)
        {
            angle -= 180f;
        }
        
        gun.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void FlipAndAdjustPosition()
    {
        // Solo actualizar si la dirección cambió
        if (wasFacingLeft != PlayerMovement.isFacingLeft)
        {
            wasFacingLeft = PlayerMovement.isFacingLeft;
            UpdateGunPosition();
        }
    }

    private void UpdateGunPosition()
    {
        if (PlayerMovement.isFacingLeft)
        {
            sr.flipX = true;
            gun.localPosition = new Vector3(-Mathf.Abs(originalLocalPosition.x), originalLocalPosition.y, originalLocalPosition.z);
        }
        else
        {
            sr.flipX = false;
            gun.localPosition = new Vector3(Mathf.Abs(originalLocalPosition.x), originalLocalPosition.y, originalLocalPosition.z);
        }
    }

    private void CheckAimingDirection()
    {
        if (player == null) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        // Determinar si está apuntando hacia atrás
        bool aimingBackwards = IsAimingBackwards(mousePos);
        
        // Si está apuntando hacia atrás y no hemos forzado el flip aún
        if (aimingBackwards && !forcedFlip)
        {
            ForcePlayerFlip();
        }
        // Si ya no apunta hacia atrás pero tenemos un flip forzado
        else if (!aimingBackwards && forcedFlip)
        {
            RestoreOriginalDirection();
        }
    }

    private bool IsAimingBackwards(Vector3 mousePos)
    {
        // Calcular el ángulo entre la dirección del player y la dirección al mouse
        Vector3 playerForward = PlayerMovement.isFacingLeft ? Vector3.left : Vector3.right;
        Vector3 aimDirection = (mousePos - transform.position).normalized;
        
        float dotProduct = Vector3.Dot(playerForward, aimDirection);
        
        // Si el producto punto es negativo, está apuntando hacia atrás
        return dotProduct < -0.1f; // Pequeño margen para evitar flip constante
    }

    private void ForcePlayerFlip()
    {
        forcedFlip = true;
        
        // Cambiar la dirección del player
        PlayerMovement.isFacingLeft = !PlayerMovement.isFacingLeft;
        
        // Actualizar el sprite del player manualmente
        if (player != null)
        {
            SpriteRenderer playerSR = player.GetComponent<SpriteRenderer>();
            if (playerSR != null)
            {
                playerSR.flipX = !playerSR.flipX;
            }
        }
        
        // Actualizar la pistola
        wasFacingLeft = PlayerMovement.isFacingLeft;
        UpdateGunPosition();
    }

    private void RestoreOriginalDirection()
    {
        forcedFlip = false;
        
        // Restaurar la dirección original del player
        // (asumiendo que la dirección "original" es la opuesta a la actual durante el aim)
        PlayerMovement.isFacingLeft = !PlayerMovement.isFacingLeft;
        
        // Actualizar el sprite del player manualmente
        if (player != null)
        {
            SpriteRenderer playerSR = player.GetComponent<SpriteRenderer>();
            if (playerSR != null)
            {
                playerSR.flipX = !playerSR.flipX;
            }
        }
        
        // Actualizar la pistola
        wasFacingLeft = PlayerMovement.isFacingLeft;
        UpdateGunPosition();
    }
}