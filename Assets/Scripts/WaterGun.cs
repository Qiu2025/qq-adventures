using UnityEngine;

public class WaterGun : MonoBehaviour
{
    [Header("Pistola de Agua")]
    [SerializeField] private Transform gun;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float resetRotationSpeed = 5f;
    [SerializeField] private ParticleSystem waterStream;

    private Quaternion gunInitialRotation;
    private SpriteRenderer gunSR;
    private SpriteRenderer playerSR;
    private Vector3 gunOriginalLocalPosition;
    private Vector3 firePointOriginalLocalPosition;
    private bool isAiming = false;

    private float nextEmitTime = 0f;
    private float emitInterval = 0.05f;

    private PlayerMovement playerMovement;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerSR = GetComponent<SpriteRenderer>();
        gunSR = gun.GetComponent<SpriteRenderer>();
        gunInitialRotation = gun.localRotation;
        gunOriginalLocalPosition = gun.localPosition;
        firePointOriginalLocalPosition = firePoint.localPosition;
        
        // Configuración inicial - SIN flips, posición derecha
        gunSR.flipX = false;
        gunSR.flipY = false;
        gun.localPosition = new Vector3(Mathf.Abs(gunOriginalLocalPosition.x), gunOriginalLocalPosition.y, gunOriginalLocalPosition.z);
        firePoint.localPosition = new Vector3(Mathf.Abs(firePointOriginalLocalPosition.x), firePointOriginalLocalPosition.y, firePointOriginalLocalPosition.z);
    }

    void Update()
    {
        CheckWaterGun();
    }

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
            
            if (Time.time >= nextEmitTime)
            {
                waterStream.Emit(2);
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
                float defaultAngle = playerMovement.isFacingRight ? 0f : 180f;
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
        
        if (!playerMovement.isFacingRight)
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
        
        float particleAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        waterStream.transform.rotation = Quaternion.Euler(0, 0, particleAngle);
    }

    private void CheckGunFlip()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        bool shouldFlip = (playerMovement.isFacingRight && mousePos.x < transform.position.x) ||
                        (!playerMovement.isFacingRight && mousePos.x > transform.position.x);
        
        if (shouldFlip)
        {
            // Cambiar dirección del player
            playerMovement.isFacingRight = !playerMovement.isFacingRight;
            
            // Actualizar animator con el valor OPUESTO al actual
            Animator animator = GetComponent<Animator>();
            bool currentAnimatorValue = animator.GetBool("isFacingRight");
            animator.SetBool("isFacingRight", !currentAnimatorValue);
            
            UpdateGunPosition();
            
            Vector3 direction = mousePos - gun.position;
            float particleAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            waterStream.transform.rotation = Quaternion.Euler(0, 0, particleAngle);
        }
    }

    public void UpdateGunPosition()
    {
        if (playerMovement.isFacingRight)
        {
            // Mirando a la derecha
            gunSR.flipX = false;
            gun.localPosition = new Vector3(Mathf.Abs(gunOriginalLocalPosition.x), gunOriginalLocalPosition.y, gunOriginalLocalPosition.z);
            firePoint.localPosition = new Vector3(Mathf.Abs(firePointOriginalLocalPosition.x), firePointOriginalLocalPosition.y, firePointOriginalLocalPosition.z);
        }
        else
        {
            // Mirando a la izquierda
            gunSR.flipX = true;
            gun.localPosition = new Vector3(-Mathf.Abs(gunOriginalLocalPosition.x), gunOriginalLocalPosition.y, gunOriginalLocalPosition.z);
            firePoint.localPosition = new Vector3(-Mathf.Abs(firePointOriginalLocalPosition.x), firePointOriginalLocalPosition.y, firePointOriginalLocalPosition.z);
        }
    }

    public bool IsAiming()
    {
        return isAiming;
    }
}