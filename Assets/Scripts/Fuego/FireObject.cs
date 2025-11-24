using UnityEngine;

public class FireObject : MonoBehaviour
{
    [Header("Configuración de Fuego")]
    [SerializeField] private bool isOnFire = true;
    [SerializeField] private int waterHitsToExtinguish = 3;
    [SerializeField] private bool destroyWhenExtinguished = false;

    private ParticleSystem fireParticles;
    private int currentWaterHits = 0;

    void Start()
    {
        fireParticles = GetComponent<ParticleSystem>();
        
        if (isOnFire)
        {
            Ignite();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && isOnFire)
        {
            KillPlayer(collision.gameObject);
        }
    }

    void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Water") && isOnFire)
        {
            HitByWater();
        }
    }

    public void HitByWater()
    {
        currentWaterHits++;
        
        if (currentWaterHits >= waterHitsToExtinguish)
        {
            Extinguish();
        }
    }

    public void Ignite()
    {
        isOnFire = true;
        currentWaterHits = 0;
        fireParticles.Play();
    }

    private void Extinguish()
    {
        isOnFire = false;
        fireParticles.Stop();
        
        if (destroyWhenExtinguished)
        {
            Destroy(gameObject, 1f);
        }
    }

    private void KillPlayer(GameObject player)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    public bool IsOnFire()
    {
        return isOnFire;
    }
}