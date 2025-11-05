using System.Collections.Generic;
using UnityEngine;

public class PoolBolas : MonoBehaviour
{
    public static PoolBolas Instance;
    public GameObject prefabBola;
    public int cantidadInicial = 10;

    private List<GameObject> pool = new List<GameObject>();

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        for (int i = 0; i < cantidadInicial; i++)
        {
            GameObject bola = Instantiate(prefabBola);
            bola.SetActive(false);
            pool.Add(bola);
        }
    }

    public GameObject ObtenerBola()
    {
        foreach (GameObject bola in pool)
        {
            if (!bola.activeInHierarchy)
                return bola;
        }

        // Si no hay bolas libres, se puede crear una nueva (opcional)
        GameObject nueva = Instantiate(prefabBola);
        nueva.SetActive(false);
        pool.Add(nueva);
        return nueva;
    }
}
