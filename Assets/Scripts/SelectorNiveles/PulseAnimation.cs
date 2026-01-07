using UnityEngine;

// Script para la animacion de latido
// Por ahora, usado solo por el nivel 1 de desierto
public class PulseAnimation : MonoBehaviour
{
    public float speed = 2f;
    public float strength = 0.05f;
    private Vector3 baseScale;

    void Start()
    {
        baseScale = transform.localScale;
    }

    void Update()
    {
        float scale = 1 + Mathf.Sin(Time.time * speed) * strength;
        transform.localScale = baseScale * scale;
    }
}