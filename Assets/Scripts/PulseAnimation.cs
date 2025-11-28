using UnityEngine;

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