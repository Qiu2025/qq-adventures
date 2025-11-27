using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class FireTile : MonoBehaviour
{
    [Header("Configuración de Fuego")]
    [SerializeField] private GameObject fireParticlePrefab; // Prefab con Particle System individual
    [SerializeField] private bool isOnFire = true;
    [SerializeField] private int waterHitsToExtinguish = 3;

    private Tilemap fireTilemap;
    private int currentWaterHits = 0;

    void Start()
    {
        fireTilemap = GetComponent<Tilemap>();

        // Crear partículas de fuego en cada tile
        
        CreateFireParticlesOnTiles();
        
    }

    private void CreateFireParticlesOnTiles()
    {
        BoundsInt bounds = fireTilemap.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);
                TileBase tile = fireTilemap.GetTile(tilePosition);

                if (tile != null) // Si hay tile en esta posición
                {
                    // Crear Particle System individual en esta posición de tile
                    Vector3 worldPosition = fireTilemap.GetCellCenterWorld(tilePosition);
                    GameObject fireParticle = Instantiate(fireParticlePrefab, worldPosition, Quaternion.identity);
                    fireParticle.transform.SetParent(transform); // Hacerlo hijo del Tilemap
                }
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Si el player toca el fuego, resetear escena
        if (collision.gameObject.CompareTag("Player") && isOnFire)
        {
            ResetScene();
        }
    }

    void OnParticleCollision(GameObject other)
    {
        // Detectar partículas de agua
        if (other.CompareTag("Water") && isOnFire)
        {
            HitByWater();
        }
    }

    public void HitByWater()
    {
        if (!isOnFire) return;

        currentWaterHits++;

        if (currentWaterHits >= waterHitsToExtinguish)
        {
            Extinguish();
        }
        else
        {
            Debug.Log($"Fuego golpeado por agua: {currentWaterHits}/{waterHitsToExtinguish}");
        }
    }

    private void Extinguish()
    {
        isOnFire = false;

        // Apagar todas las partículas de fuego hijas
        foreach (Transform child in transform)
        {
            ParticleSystem childParticles = child.GetComponent<ParticleSystem>();
            if (childParticles != null)
            {
                childParticles.Stop();
            }
        }

        Debug.Log("Fuego completamente apagado!");
    }

    private void ResetScene()
    {
        Debug.Log("¡Player quemado! Reseteando escena...");

        // Recargar la escena actual
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public bool IsOnFire()
    {
        return isOnFire;
    }





}