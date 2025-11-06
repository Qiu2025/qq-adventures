using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PoolBolasPlaymodeTests
{
    private GameObject poolGO;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        var prefab = Resources.Load<GameObject>("BulletPool");
        Assert.NotNull(prefab, "No encontré el prefab 'BulletPool' en Resources.");
        poolGO = Object.Instantiate(prefab);
        yield return null; // deja correr Awake/Start
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        if (poolGO) Object.Destroy(poolGO);
        yield return null;
    }

    [UnityTest]
    public IEnumerator EntregaInactivas_Y_CreceSiSeAgotan()
    {
        var pool = PoolBolas.Instance;
        Assert.IsNotNull(pool, "PoolBolas.Instance es null (¿tiene el componente en BulletPool?).");
        Assert.IsNotNull(pool.prefabBola, "PoolBolas.prefabBola no está asignado en el inspector de BulletPool.");

        var b1 = pool.ObtenerBola();
        var b2 = pool.ObtenerBola();
        Assert.IsFalse(b1.activeInHierarchy, "El pool debe entregar bolas INACTIVAS.");
        Assert.IsFalse(b2.activeInHierarchy, "El pool debe entregar bolas INACTIVAS.");

        // Simula uso
        b1.SetActive(true);
        b2.SetActive(true);

        // Debe crear/servir una tercera bola inactiva
        var b3 = pool.ObtenerBola();
        Assert.IsFalse(b3.activeInHierarchy, "Las nuevas también deben venir inactivas.");
        Assert.AreNotSame(b1, b3);
        Assert.AreNotSame(b2, b3);
        yield return null;
    }
}
