using UnityEngine;

public class Fire : MonoBehaviour
{
    [Header("Settings")]
    public float intensity = 1f;           // 1 = fuego completo, 0 = apagado
    public float extinguishThreshold = 0.1f;
    public ParticleSystem fireParticles;   // efecto visual del fuego

    private bool isOut = false;

    void Start()
    {
        if (fireParticles == null)
            fireParticles = GetComponentInChildren<ParticleSystem>();
    }

    public void Extinguish(float amount)
    {
        if (isOut) return;

        intensity -= amount;
        intensity = Mathf.Clamp01(intensity);

        if (fireParticles != null)
        {
            var main = fireParticles.main;
            main.startSize = Mathf.Lerp(0.1f, 1f, intensity);
            main.startColor = Color.Lerp(Color.gray, Color.red, intensity);
        }

        if (intensity <= extinguishThreshold)
        {
            isOut = true;
            if (fireParticles != null)
                fireParticles.Stop();
            // puedes destruir el objeto o cambiar sprite
            // Destroy(gameObject);
        }
    }
}
