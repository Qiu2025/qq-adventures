using UnityEngine;

public class BackgroundFollow : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float parallaxX = 0f;   // 0 = no movimiento, 1 = se mueve igual que la cámara
    [SerializeField] private float parallaxY = 0f;
    
    Vector3 initialCameraPos;
    Vector3 initialPosition;

    void Start()
    {
        if (cameraTransform == null) cameraTransform = Camera.main.transform;
        initialCameraPos = cameraTransform.position;
        initialPosition = transform.position;
    }

    void LateUpdate()
    {
        Vector3 camDelta = cameraTransform.position - initialCameraPos;
        Vector3 newPos = initialPosition + new Vector3(camDelta.x * parallaxX, camDelta.y * parallaxY, 0f);
        transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
    }
}
