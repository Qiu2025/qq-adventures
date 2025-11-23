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
    private Vector3 gunOriginalLocalPosition;
    private Vector3 firePointOriginalLocalPosition;
    private bool isAiming = false;

    private float nextEmitTime = 0f;
    private float emitInterval = 0.05f;

    void Start()
    {
        gunSR = gun.GetComponent<SpriteRenderer>();
        gunInitialRotation = gun.localRotation;
        gunOriginalLocalPosition = gun.localPosition;
        firePointOriginalLocalPosition = firePoint.localPosition;
        
        // Forzar que al inicio mire a la izquierda
        PlayerMovement.isFacingLeft = true;
        UpdateGunPosition();
        
        // Configurar rotación inicial para mirar a la izquierda
        gun.rotation = Quaternion.Euler(0, 0, 180f);
        firePoint.rotation = Quaternion.Euler(0, 0, 180f);
        waterStream.transform.rotation = Quaternion.Euler(0, 0, 180f);
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
                float defaultAngle = PlayerMovement.isFacingLeft ? 180f : 0f;
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
        
        if (PlayerMovement.isFacingLeft)
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
        
        bool aimingBackwards = (mousePos.x < transform.position.x && !PlayerMovement.isFacingLeft) ||
                        (mousePos.x > transform.position.x && PlayerMovement.isFacingLeft);
        
        if (aimingBackwards)
        {
            PlayerMovement.isFacingLeft = !PlayerMovement.isFacingLeft;
            
            SpriteRenderer playerSR = GetComponent<SpriteRenderer>();
            if (playerSR != null)
            {
                playerSR.flipX = !playerSR.flipX;
            }
            
            UpdateGunPosition();
            
            Vector3 direction = mousePos - gun.position;
            float particleAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            waterStream.transform.rotation = Quaternion.Euler(0, 0, particleAngle);
        }
    }

    public void UpdateGunPosition()
    {
        if (PlayerMovement.isFacingLeft)
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

    public bool IsAiming()
    {
        return isAiming;
    }
}